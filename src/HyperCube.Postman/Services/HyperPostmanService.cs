using System.Collections.Concurrent;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks.Dataflow;
using HyperCube.Postman.Config;
using HyperCube.Postman.Interfaces.Events;
using HyperCube.Postman.Interfaces.Services;
using HyperCube.Postman.Internal;
using HyperCube.Postman.Wraps;
using Microsoft.Extensions.Logging;

namespace HyperCube.Postman.Services;

/// <summary>
///     Default implementation of the HyperPostman event dispatch system.
/// </summary>
public class HyperPostmanService : IHyperPostmanService, IDisposable
{
    private readonly ILogger _logger;
    private readonly ConcurrentDictionary<Type, object> _listeners = new();
    private readonly ActionBlock<EventDispatchJob> _dispatchBlock;
    private readonly CancellationTokenSource _cts = new();

    private readonly Subject<object> _allEventsSubject = new Subject<object>();

    /// <summary>();
    /// Observable  that emits all events
    /// </summary>
    public IObservable<object> AllEventsObservable => _allEventsSubject;

    public HyperPostmanService(ILogger<HyperPostmanService> logger, HyperPostmanConfig config)
    {
        _logger = logger;
        var executionOptions = new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = config.MaxConcurrentTasks,
            CancellationToken = _cts.Token
        };

        _dispatchBlock = new ActionBlock<EventDispatchJob>(
            job => job.ExecuteAsync(),
            executionOptions
        );

        _logger.LogInformation(
            "Signal emitter initialized with {ParallelTasks} dispatch tasks",
            config.MaxConcurrentTasks
        );
    }

    /// <summary>
    /// Register a listener for a specific event type
    /// </summary>
    public void Subscribe<TEvent>(ILetterListener<TEvent> listener) where TEvent : class, IHyperPostmanEvent
    {
        var eventType = typeof(TEvent);

        // Get or create a list of listeners for this event type
        var listeners = (ConcurrentBag<ILetterListener<TEvent>>)_listeners.GetOrAdd(
            eventType,
            _ => new ConcurrentBag<ILetterListener<TEvent>>()
        );

        listeners.Add(listener);

        _logger.LogTrace(
            "Registered listener {ListenerType} for event {EventType}",
            listener.GetType().Name,
            eventType.Name
        );
    }

    /// <summary>
    /// Register a function as a listener for a specific event type
    /// </summary>
    public void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class, IHyperPostmanEvent
    {
        var listener = new FunctionSignalListener<TEvent>(handler);
        Subscribe<TEvent>(listener);

        _logger.LogTrace(
            "Registered function handler for event {EventType}",
            typeof(TEvent).Name
        );
    }

    /// <summary>
    /// Unregisters a listener for a specific event type
    /// </summary>
    public void Unsubscribe<TEvent>(ILetterListener<TEvent> listener)
        where TEvent : class, IHyperPostmanEvent
    {
        var eventType = typeof(TEvent);

        if (_listeners.TryGetValue(eventType, out var listenersObj))
        {
            var listeners = (ConcurrentBag<ILetterListener<TEvent>>)listenersObj;

            // Create a new bag without the listener
            var updatedListeners = new ConcurrentBag<ILetterListener<TEvent>>(
                listeners.Where(l => !ReferenceEquals(l, listener))
            );

            _listeners.TryUpdate(eventType, updatedListeners, listeners);

            _logger.LogTrace(
                "Unregistered listener {ListenerType} from event {EventType}",
                listener.GetType().Name,
                eventType.Name
            );
        }
    }

    /// <summary>
    /// Unregisters a function handler for a specific event type
    /// </summary>
    public void Unsubscribe<TEvent>(Func<TEvent, Task> handler)
        where TEvent : class, IHyperPostmanEvent
    {
        var eventType = typeof(TEvent);

        if (_listeners.TryGetValue(eventType, out var listenersObj))
        {
            var listeners = (ConcurrentBag<ILetterListener<TEvent>>)listenersObj;

            // Create a new bag without the function handler
            var updatedListeners = new ConcurrentBag<ILetterListener<TEvent>>(
                listeners.Where(
                    l => !(l is FunctionSignalListener<TEvent> functionListener) ||
                         !functionListener.HasSameHandler(handler)
                )
            );

            _listeners.TryUpdate(eventType, updatedListeners, listeners);

            _logger.LogTrace(
                "Unregistered function handler for event {EventType}",
                eventType.Name
            );
        }
    }

    /// <summary>
    /// Emits an event to all registered listeners asynchronously
    /// </summary>
    public async Task PublishAsync<TEvent>(TEvent eventData, CancellationToken cancellationToken = default)
        where TEvent : class, IHyperPostmanEvent
    {
        var eventType = typeof(TEvent);

        _allEventsSubject.OnNext(eventData);

        if (!_listeners.TryGetValue(eventType, out var listenersObj))
        {
            _logger.LogTrace("No listeners registered for event {EventType}", eventType.Name);
            return;
        }

        var listeners = (ConcurrentBag<ILetterListener<TEvent>>)listenersObj;

        _logger.LogTrace(
            "Emitting event {EventType} to {ListenerCount} listeners",
            eventType.Name,
            listeners.Count
        );

        // Dispatch jobs to process the event for each listener
        foreach (var listener in listeners)
        {
            var job = new EventDispatchJob<TEvent>(listener, eventData);
            await _dispatchBlock.SendAsync(job, cancellationToken);
        }
    }

    public int GetListenerCount()
    {
        var totalCount = 0;

        foreach (var kvp in _listeners)
        {
            var listeners = (ConcurrentBag<ILetterListener<IHyperPostmanEvent>>)kvp.Value;
            totalCount += listeners.Count;
        }

        return totalCount;
    }

    public int GetListenerCount<TEvent>() where TEvent : IHyperPostmanEvent
    {
        if (_listeners.TryGetValue(typeof(TEvent), out var listenersObj))
        {
            var listeners = (ConcurrentBag<ILetterListener<TEvent>>)listenersObj;
            return listeners.Count;
        }

        return 0;
    }

    /// <summary>
    /// Waits for all queued events to be processed
    /// </summary>
    public async Task WaitForCompletionAsync()
    {
        _dispatchBlock.Complete();
        await _dispatchBlock.Completion;
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
