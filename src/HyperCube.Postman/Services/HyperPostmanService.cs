using System.Collections.Concurrent;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks.Dataflow;
using HyperCube.Postman.Config;
using HyperCube.Postman.Interfaces.Events;
using HyperCube.Postman.Interfaces.Services;
using HyperCube.Postman.Wraps;
using Microsoft.Extensions.Logging;

namespace HyperCube.Postman.Services;

/// <summary>
///     Default implementation of the HyperPostman event dispatch system.
/// </summary>
public class HyperPostmanService : IHyperPostmanService, IDisposable
{
    private readonly Subject<object> _allEventsSubject = new();
    private readonly HyperPostmanConfig _config;
    private readonly ActionBlock<(Type eventType, IHyperPostmanEvent @event, CancellationToken token)> _dispatchQueue;
    private readonly SemaphoreSlim _dispatchSemaphore;
    private readonly ConcurrentDictionary<Type, List<object>> _listeners = new();
    private readonly ILogger<HyperPostmanService> _logger;
    private bool _isDisposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HyperPostmanService" /> class.
    /// </summary>
    /// <param name="logger">The logger to use for logging events and errors.</param>
    /// <param name="options">The configuration options for the postman service.</param>
    public HyperPostmanService(
        ILogger<HyperPostmanService> logger, HyperPostmanConfig options
    )
    {
        _logger = logger;
        _config = options;

        // Create a semaphore to limit concurrent dispatches
        var maxTasks = _config.MaxConcurrentTasks <= 0
            ? Environment.ProcessorCount
            : _config.MaxConcurrentTasks;

        _dispatchSemaphore = new SemaphoreSlim(maxTasks, maxTasks);

        // Configure the dispatch queue
        var dispatchOptions = new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = maxTasks,
            BoundedCapacity = _config.BufferEvents ? _config.MaxBufferSize : 1
        };

        _dispatchQueue = new ActionBlock<(Type eventType, IHyperPostmanEvent @event, CancellationToken token)>(
            ProcessEventAsync,
            dispatchOptions
        );

        _logger.LogInformation(
            "HyperPostman service initialized with {MaxTasks} concurrent tasks and buffer capacity {BufferCapacity}",
            maxTasks,
            _config.BufferEvents ? _config.MaxBufferSize : 1
        );
    }

    /// <summary>
    ///     Disposes resources used by the HyperPostman service.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _dispatchQueue.Complete();
        _dispatchSemaphore.Dispose();
        _isDisposed = true;

        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Observable  that emits all events
    /// </summary>
    public IObservable<object> AllEventsObservable => _allEventsSubject;

    /// <inheritdoc />
    public void RegisterListener<TEvent>(ILetterListener<TEvent> listener) where TEvent : IHyperPostmanEvent
    {
        if (listener == null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        var eventType = typeof(TEvent);
        var listeners = _listeners.GetOrAdd(eventType, _ => new List<object>());

        lock (listeners)
        {
            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
                _logger.LogDebug(
                    "Registered listener {ListenerType} for event {EventType}",
                    listener.GetType().Name,
                    eventType.Name
                );
            }
        }
    }

    /// <inheritdoc />
    public IDisposable RegisterCallback<TEvent>(Func<TEvent, CancellationToken, Task> callback)
        where TEvent : IHyperPostmanEvent
    {
        if (callback == null)
        {
            throw new ArgumentNullException(nameof(callback));
        }

        var wrapper = new CallbackWrapper<TEvent>(callback);
        RegisterListener(wrapper);

        return new SubscriptionDisposable(() => UnregisterListener(wrapper));
    }

    /// <inheritdoc />
    public void UnregisterListener<TEvent>(ILetterListener<TEvent> listener) where TEvent : IHyperPostmanEvent
    {
        if (listener == null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        var eventType = typeof(TEvent);

        if (_listeners.TryGetValue(eventType, out var listeners))
        {
            lock (listeners)
            {
                if (listeners.Remove(listener))
                {
                    _logger.LogDebug(
                        "Unregistered listener {ListenerType} for event {EventType}",
                        listener.GetType().Name,
                        eventType.Name
                    );
                }
            }
        }
    }

    /// <inheritdoc />
    public int GetListenerCount()
    {
        return _listeners.Values.Sum(list => list.Count);
    }

    /// <inheritdoc />
    public int GetListenerCount<TEvent>() where TEvent : IHyperPostmanEvent
    {
        var eventType = typeof(TEvent);
        return _listeners.TryGetValue(eventType, out var listeners) ? listeners.Count : 0;
    }

    /// <inheritdoc />
    public async Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IHyperPostmanEvent
    {
        _allEventsSubject.OnNext(@event);

        if (@event == null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        var eventType = typeof(TEvent);

        if (!_listeners.TryGetValue(eventType, out _) || GetListenerCount<TEvent>() == 0)
        {
            _logger.LogDebug("No listeners registered for event {EventType}", eventType.Name);
            return;
        }

        _logger.LogDebug(
            "Queueing event {EventType} ({EventId}) for dispatch",
            eventType.Name,
            @event.Id
        );

        var queued = await _dispatchQueue.SendAsync(
            (eventType, @event, cancellationToken),
            cancellationToken
        );

        if (!queued && _config.BufferEvents)
        {
            _logger.LogWarning(
                "Failed to queue event {EventType} ({EventId}) for dispatch - buffer full",
                eventType.Name,
                @event.Id
            );
        }
    }

    /// <summary>
    ///     Processes an event by dispatching it to all registered listeners.
    /// </summary>
    private async Task ProcessEventAsync((Type eventType, IHyperPostmanEvent @event, CancellationToken token) eventData)
    {
        var (eventType, @event, cancellationToken) = eventData;

        try
        {
            await _dispatchSemaphore.WaitAsync(cancellationToken);

            try
            {
                _logger.LogDebug(
                    "Processing event {EventType} ({EventId})",
                    eventType.Name,
                    @event.Id
                );

                if (!_listeners.TryGetValue(eventType, out var listeners) || listeners.Count == 0)
                {
                    _logger.LogDebug("No listeners registered for event {EventType}", eventType.Name);
                    return;
                }

                // Create a local copy of the listeners to avoid issues if the collection changes during dispatching
                List<object> listenersCopy;
                lock (listeners)
                {
                    listenersCopy = listeners.ToList();
                }

                _logger.LogDebug(
                    "Dispatching event {EventType} ({EventId}) to {ListenerCount} listeners",
                    eventType.Name,
                    @event.Id,
                    listenersCopy.Count
                );

                var tasks = new List<Task>();
                var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                if (_config.TimeoutMilliseconds > 0)
                {
                    cancellationSource.CancelAfter(_config.TimeoutMilliseconds);
                }

                var linkedToken = cancellationSource.Token;

                // Create a dispatcher method for the specific event type using reflection
                var dispatchMethod = typeof(HyperPostmanService)
                    .GetMethod(
                        "DispatchToListener",
                        BindingFlags.NonPublic | BindingFlags.Instance
                    )
                    ?.MakeGenericMethod(eventType);

                if (dispatchMethod == null)
                {
                    _logger.LogError("Failed to create dispatch method for event {EventType}", eventType.Name);
                    return;
                }

                foreach (var listener in listenersCopy)
                {
                    var task = (Task)dispatchMethod.Invoke(this, new[] { listener, @event, linkedToken })!;
                    tasks.Add(task);
                }

                try
                {
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                    _logger.LogDebug("Completed dispatch of event {EventType} ({EventId})", eventType.Name, @event.Id);
                }
                catch (Exception ex) when (ex is OperationCanceledException or TaskCanceledException)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _logger.LogWarning(
                            "Dispatch of event {EventType} ({EventId}) was cancelled by caller",
                            eventType.Name,
                            @event.Id
                        );
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Dispatch of event {EventType} ({EventId}) timed out after {Timeout}ms",
                            eventType.Name,
                            @event.Id,
                            _config.TimeoutMilliseconds
                        );
                    }
                }
                finally
                {
                    if (!cancellationSource.IsCancellationRequested)
                    {
                        // Ensure we dispose the token source
                        try
                        {
                            cancellationSource.Cancel();
                        }
                        catch
                        {
                        }
                    }

                    cancellationSource.Dispose();
                }
            }
            finally
            {
                _dispatchSemaphore.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error in event dispatch engine for event {EventType} ({EventId})",
                eventType.Name,
                @event.Id
            );

            if (!_config.ContinueOnError)
            {
                throw;
            }
        }
    }

    /// <summary>
    ///     Dispatches an event to a specific listener with the correct type.
    /// </summary>
    private async Task DispatchToListener<TEvent>(
        object listener, IHyperPostmanEvent baseEvent, CancellationToken cancellationToken
    )
        where TEvent : IHyperPostmanEvent
    {
        var typedEvent = (TEvent)baseEvent;
        var typedListener = listener as ILetterListener<TEvent>;

        if (typedListener == null)
        {
            _logger.LogWarning(
                "Listener {ListenerType} is not compatible with event {EventType}",
                listener.GetType().Name,
                typeof(TEvent).Name
            );
            return;
        }

        try
        {
            await typedListener.HandleAsync(typedEvent, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is OperationCanceledException or TaskCanceledException)
        {
            _logger.LogWarning(
                "Listener {ListenerType} handling of event {EventType} ({EventId}) was cancelled",
                typedListener.GetType().Name,
                typeof(TEvent).Name,
                typedEvent.Id
            );

            if (!_config.ContinueOnError)
            {
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error in listener {ListenerType} handling event {EventType} ({EventId})",
                typedListener.GetType().Name,
                typeof(TEvent).Name,
                typedEvent.Id
            );

            if (!_config.ContinueOnError)
            {
                throw;
            }
        }
    }
}
