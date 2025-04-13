using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using HyperCube.Queue.Core.Data.Config;
using HyperCube.Queue.Core.Interfaces;
using HyperCube.Queue.Core.Interfaces.Providers;
using HyperCube.Queue.Core.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HyperCube.Queue.Core.Providers;

/// <summary>
/// In-memory implementation of the queue provider.
/// </summary>
public class InMemoryQueueProvider : IQueueProvider, IDisposable
{
    private readonly ILogger<InMemoryQueueProvider> _logger;
    private readonly QueueConfig _config;
    private readonly ConcurrentDictionary<string, ConcurrentQueue<QueueMessageWrapper>> _queues = new();

    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, QueueMessageWrapper>> _processingMessages =
        new();

    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, IDisposable>> _subscriptions = new();
    private readonly ConcurrentDictionary<string, Subject<QueueMessageWrapper>> _subjects = new();
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _queueCancellations = new();
    private readonly Timer _scheduledMessagesTimer;
    private readonly ConcurrentQueue<ScheduledMessage> _scheduledMessages = new();
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryQueueProvider"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The queue configuration options.</param>
    public InMemoryQueueProvider(
        ILogger<InMemoryQueueProvider> logger,
        IOptions<QueueConfig> options
    )
    {
        _logger = logger;
        _config = options.Value;
        _scheduledMessagesTimer = new Timer(
            ProcessScheduledMessages,
            null,
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1)
        );
    }

    /// <inheritdoc />
    public string ProviderType => "InMemory";

    /// <inheritdoc />
    public Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Initializing InMemory queue provider");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task CreateQueueAsync(string queueName, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating queue {QueueName}", queueName);

        _queues.TryAdd(queueName, new ConcurrentQueue<QueueMessageWrapper>());
        _processingMessages.TryAdd(queueName, new ConcurrentDictionary<string, QueueMessageWrapper>());
        _subscriptions.TryAdd(queueName, new ConcurrentDictionary<string, IDisposable>());
        _subjects.TryAdd(queueName, new Subject<QueueMessageWrapper>());
        _queueCancellations.TryAdd(queueName, new CancellationTokenSource());

        StartProcessingQueue(queueName);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteQueueAsync(string queueName, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting queue {QueueName}", queueName);

        if (_queueCancellations.TryRemove(queueName, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
        }

        if (_subjects.TryRemove(queueName, out var subject))
        {
            subject.OnCompleted();
            subject.Dispose();
        }

        if (_subscriptions.TryRemove(queueName, out var subs))
        {
            foreach (var sub in subs.Values)
            {
                sub.Dispose();
            }
        }

        _queues.TryRemove(queueName, out _);
        _processingMessages.TryRemove(queueName, out _);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<bool> QueueExistsAsync(string queueName, CancellationToken cancellationToken = default)
    {
        var exists = _queues.ContainsKey(queueName);
        return Task.FromResult(exists);
    }

    /// <inheritdoc />
    public Task<IEnumerable<string>> GetQueuesAsync(CancellationToken cancellationToken = default)
    {
        var queues = _queues.Keys.ToList().AsEnumerable();
        return Task.FromResult(queues);
    }

    /// <inheritdoc />
    public Task PublishAsync<TPayload>(
        string queueName, QueueMessage<TPayload> message, CancellationToken cancellationToken = default
    )
        where TPayload : class
    {
        _logger.LogDebug("Publishing message {MessageId} to queue {QueueName}", message.Id, queueName);

        if (message.ScheduledDelivery.HasValue && message.ScheduledDelivery.Value > DateTime.UtcNow)
        {
            // Schedule the message for later delivery
            _scheduledMessages.Enqueue(
                new ScheduledMessage
                {
                    QueueName = queueName,
                    Message = new QueueMessageWrapper
                    {
                        Id = message.Id,
                        MessageType = typeof(TPayload).FullName!,
                        Content = JsonSerializer.Serialize(message)
                    },
                    DeliveryTime = message.ScheduledDelivery.Value
                }
            );

            return Task.CompletedTask;
        }

        // Ensure the queue exists
        if (!_queues.ContainsKey(queueName) && _config.AutoCreateQueues)
        {
            CreateQueueAsync(queueName, cancellationToken).GetAwaiter().GetResult();
        }

        if (!_queues.TryGetValue(queueName, out var queue))
        {
            throw new InvalidOperationException($"Queue {queueName} does not exist.");
        }

        var wrapper = new QueueMessageWrapper
        {
            Id = message.Id,
            MessageType = typeof(TPayload).FullName!,
            Content = JsonSerializer.Serialize(message)
        };

        queue.Enqueue(wrapper);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public IDisposable Subscribe<TPayload>(
        string queueName, Func<QueueMessage<TPayload>, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default
    )
        where TPayload : class
    {
        _logger.LogDebug("Subscribing to queue {QueueName}", queueName);

        // Ensure the queue exists
        if (!_queues.ContainsKey(queueName) && _config.AutoCreateQueues)
        {
            CreateQueueAsync(queueName, cancellationToken).GetAwaiter().GetResult();
        }

        if (!_subjects.TryGetValue(queueName, out var subject))
        {
            throw new InvalidOperationException($"Queue {queueName} does not exist.");
        }

        var subscription = subject
            .Where(wrapper => wrapper.MessageType == typeof(TPayload).FullName)
            .Subscribe(
                async wrapper =>
                {
                    try
                    {
                        var message = JsonSerializer.Deserialize<QueueMessage<TPayload>>(wrapper.Content);
                        if (message != null)
                        {
                            await handler(message, cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Error handling message {MessageId} from queue {QueueName}",
                            wrapper.Id,
                            queueName
                        );
                    }
                }
            );

        var subscriptionId = Guid.NewGuid().ToString();
        _subscriptions[queueName].TryAdd(subscriptionId, subscription);

        return new SubscriptionDisposable(
            () =>
            {
                if (_subscriptions.TryGetValue(queueName, out var subs))
                {
                    if (subs.TryRemove(subscriptionId, out var sub))
                    {
                        sub.Dispose();
                    }
                }
            }
        );
    }

    /// <inheritdoc />
    public Task AcknowledgeAsync(string queueName, string messageId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Acknowledging message {MessageId} from queue {QueueName}", messageId, queueName);

        if (_processingMessages.TryGetValue(queueName, out var processing))
        {
            processing.TryRemove(messageId, out _);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RejectAsync(
        string queueName, string messageId, bool requeue = false, CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Rejecting message {MessageId} from queue {QueueName}, requeue: {Requeue}",
            messageId,
            queueName,
            requeue
        );

        if (_processingMessages.TryGetValue(queueName, out var processing))
        {
            if (processing.TryRemove(messageId, out var message) && requeue)
            {
                // Requeue the message
                if (_queues.TryGetValue(queueName, out var queue))
                {
                    queue.Enqueue(message);
                }
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<int> GetMessageCountAsync(string queueName, CancellationToken cancellationToken = default)
    {
        if (_queues.TryGetValue(queueName, out var queue))
        {
            return Task.FromResult(queue.Count);
        }

        return Task.FromResult(0);
    }

    /// <inheritdoc />
    public Task PurgeQueueAsync(string queueName, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Purging queue {QueueName}", queueName);

        if (_queues.TryGetValue(queueName, out var queue))
        {
            while (queue.TryDequeue(out _))
            {
                // Just dequeue all messages
            }
        }

        if (_processingMessages.TryGetValue(queueName, out var processing))
        {
            processing.Clear();
        }

        return Task.CompletedTask;
    }

    private void StartProcessingQueue(string queueName)
    {
        _logger.LogDebug("Starting processing for queue {QueueName}", queueName);

        Task.Run(
            async () =>
            {
                var cts = _queueCancellations[queueName];

                try
                {
                    while (!cts.Token.IsCancellationRequested)
                    {
                        if (_queues.TryGetValue(queueName, out var queue) &&
                            _subjects.TryGetValue(queueName, out var subject) &&
                            _processingMessages.TryGetValue(queueName, out var processing))
                        {
                            if (queue.TryDequeue(out var message))
                            {
                                // Add to processing messages
                                processing.TryAdd(message.Id, message);

                                // Notify subscribers
                                subject.OnNext(message);
                            }
                            else
                            {
                                // No messages, wait a bit
                                await Task.Delay(100, cts.Token);
                            }
                        }
                        else
                        {
                            // Queue not found, wait a bit
                            await Task.Delay(100, cts.Token);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Normal cancellation, just exit
                    _logger.LogDebug("Processing for queue {QueueName} was cancelled", queueName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing queue {QueueName}", queueName);
                }
            }
        );
    }

    private void ProcessScheduledMessages(object? state)
    {
        var now = DateTime.UtcNow;
        var count = 0;

        try
        {
            // Check for scheduled messages that are due
            var messagesToProcess = new List<ScheduledMessage>();

            while (_scheduledMessages.TryPeek(out var message) && message.DeliveryTime <= now)
            {
                if (_scheduledMessages.TryDequeue(out message))
                {
                    messagesToProcess.Add(message);
                }
            }

            // Process due messages
            foreach (var message in messagesToProcess)
            {
                if (_queues.TryGetValue(message.QueueName, out var queue))
                {
                    queue.Enqueue(message.Message);
                    count++;
                }
            }

            if (count > 0)
            {
                _logger.LogDebug("Processed {Count} scheduled messages", count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing scheduled messages");
        }
    }

    /// <summary>
    /// Disposes the resources used by the provider.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _scheduledMessagesTimer.Dispose();

        foreach (var cts in _queueCancellations.Values)
        {
            cts.Cancel();
            cts.Dispose();
        }

        foreach (var subject in _subjects.Values)
        {
            subject.OnCompleted();
            subject.Dispose();
        }

        foreach (var subs in _subscriptions.Values)
        {
            foreach (var sub in subs.Values)
            {
                sub.Dispose();
            }
        }

        _isDisposed = true;
    }

    /// <summary>
    /// Wrapper class for queue messages to store metadata.
    /// </summary>
    private class QueueMessageWrapper
    {
        /// <summary>
        /// Gets or sets the message ID.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the message type.
        /// </summary>
        public string MessageType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the serialized message content.
        /// </summary>
        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// Class for scheduled messages.
    /// </summary>
    private class ScheduledMessage
    {
        /// <summary>
        /// Gets or sets the queue name.
        /// </summary>
        public string QueueName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public QueueMessageWrapper Message { get; set; } = new();

        /// <summary>
        /// Gets or sets the delivery time.
        /// </summary>
        public DateTime DeliveryTime { get; set; }
    }

    /// <summary>
    /// Disposable class for subscription cleanup.
    /// </summary>
    private class SubscriptionDisposable : IDisposable
    {
        private readonly Action _disposeAction;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionDisposable"/> class.
        /// </summary>
        /// <param name="disposeAction">The action to perform when disposing.</param>
        public SubscriptionDisposable(Action disposeAction)
        {
            _disposeAction = disposeAction;
        }

        /// <summary>
        /// Disposes the subscription.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposeAction();
            _disposed = true;
        }
    }
}
