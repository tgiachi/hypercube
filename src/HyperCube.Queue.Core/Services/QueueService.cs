using System.Reactive.Linq;
using System.Reactive.Subjects;
using HyperCube.Queue.Core.Data.Config;
using HyperCube.Queue.Core.Interfaces.Listeners;
using HyperCube.Queue.Core.Interfaces.Providers;
using HyperCube.Queue.Core.Interfaces.Services;
using HyperCube.Queue.Core.Messages;
using Microsoft.Extensions.Logging;

namespace HyperCube.Queue.Core.Services;

/// <summary>
/// Default implementation of the queue service.
/// </summary>
public class QueueService : IQueueService
{
    private readonly IQueueProvider _provider;
    private readonly QueueConfig _config;
    private readonly ILogger<QueueService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueService"/> class.
    /// </summary>
    /// <param name="provider">The queue provider.</param>
    /// <param name="config">The queue configuration.</param>
    /// <param name="logger">The logger.</param>
    public QueueService(
        IQueueProvider provider,
        QueueConfig config,
        ILogger<QueueService> logger)
    {
        _provider = provider;
        _config = config;
        _logger = logger;
    }

    /// <inheritdoc />
    public IQueuePublisher<TPayload> CreatePublisher<TPayload>(string? queueName = null) where TPayload : class
    {
        var queue = GetQueueName<TPayload>(queueName);
        return new QueuePublisher<TPayload>(queue, _provider, _logger);
    }

    /// <inheritdoc />
    public IQueueListener<TPayload> CreateListener<TPayload>(string? queueName = null) where TPayload : class
    {
        var queue = GetQueueName<TPayload>(queueName);
        return new QueueListener<TPayload>(queue, _provider, _logger);
    }

    /// <inheritdoc />
    public Task PublishAsync<TPayload>(QueueMessage<TPayload> message, string? queueName = null, CancellationToken cancellationToken = default)
        where TPayload : class
    {
        var queue = GetQueueName<TPayload>(queueName);
        return _provider.PublishAsync(queue, message, cancellationToken);
    }

    /// <inheritdoc />
    public Task PublishAsync<TPayload>(TPayload payload, string? queueName = null, CancellationToken cancellationToken = default)
        where TPayload : class
    {
        var message = new QueueMessage<TPayload>(payload);
        return PublishAsync(message, queueName, cancellationToken);
    }

    /// <inheritdoc />
    public IDisposable Subscribe<TPayload>(Func<QueueMessage<TPayload>, CancellationToken, Task> handler, string? queueName = null, CancellationToken cancellationToken = default)
        where TPayload : class
    {
        var queue = GetQueueName<TPayload>(queueName);
        return _provider.Subscribe(queue, handler, cancellationToken);
    }

    private string GetQueueName<TPayload>(string? queueName = null)
    {
        if (!string.IsNullOrEmpty(queueName))
        {
            return _config.QueuePrefix + queueName;
        }

        return _config.QueuePrefix + typeof(TPayload).Name.ToLowerInvariant();
    }
}

/// <summary>
/// Default implementation of the queue publisher.
/// </summary>
/// <typeparam name="TPayload">The type of the message payload.</typeparam>
public class QueuePublisher<TPayload> : IQueuePublisher<TPayload> where TPayload : class
{
    private readonly IQueueProvider _provider;
    private readonly ILogger _logger;

    /// <inheritdoc />
    public string QueueName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueuePublisher{TPayload}"/> class.
    /// </summary>
    /// <param name="queueName">The name of the queue.</param>
    /// <param name="provider">The queue provider.</param>
    /// <param name="logger">The logger.</param>
    public QueuePublisher(
        string queueName,
        IQueueProvider provider,
        ILogger logger)
    {
        QueueName = queueName;
        _provider = provider;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task PublishAsync(QueueMessage<TPayload> message, CancellationToken cancellationToken = default)
    {
        return _provider.PublishAsync(QueueName, message, cancellationToken);
    }

    /// <inheritdoc />
    public Task PublishAsync(TPayload payload, CancellationToken cancellationToken = default)
    {
        var message = new QueueMessage<TPayload>(payload);
        return PublishAsync(message, cancellationToken);
    }

    /// <inheritdoc />
    public Task ScheduleAsync(QueueMessage<TPayload> message, TimeSpan delay, CancellationToken cancellationToken = default)
    {
        message.ScheduledDelivery = DateTime.UtcNow.Add(delay);
        return PublishAsync(message, cancellationToken);
    }

    /// <inheritdoc />
    public Task ScheduleAsync(TPayload payload, TimeSpan delay, CancellationToken cancellationToken = default)
    {
        var message = new QueueMessage<TPayload>(payload)
        {
            ScheduledDelivery = DateTime.UtcNow.Add(delay)
        };

        return PublishAsync(message, cancellationToken);
    }

    /// <inheritdoc />
    public Task ScheduleAsync(QueueMessage<TPayload> message, DateTime scheduledTime, CancellationToken cancellationToken = default)
    {
        message.ScheduledDelivery = scheduledTime;
        return PublishAsync(message, cancellationToken);
    }

    /// <inheritdoc />
    public Task ScheduleAsync(TPayload payload, DateTime scheduledTime, CancellationToken cancellationToken = default)
    {
        var message = new QueueMessage<TPayload>(payload)
        {
            ScheduledDelivery = scheduledTime
        };

        return PublishAsync(message, cancellationToken);
    }
}

/// <summary>
/// Default implementation of the queue listener.
/// </summary>
/// <typeparam name="TPayload">The type of the message payload.</typeparam>
public class QueueListener<TPayload> : IQueueListener<TPayload> where TPayload : class
{
    private readonly IQueueProvider _provider;
    private readonly ILogger _logger;
    private readonly Subject<QueueMessage<TPayload>> _subject = new();
    private readonly CancellationTokenSource _cts = new();
    private IDisposable? _subscription;
    private bool _isStarted;

    /// <inheritdoc />
    public string QueueName { get; }

    /// <inheritdoc />
    public IObservable<QueueMessage<TPayload>> Messages => _subject.AsObservable();

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueListener{TPayload}"/> class.
    /// </summary>
    /// <param name="queueName">The name of the queue.</param>
    /// <param name="provider">The queue provider.</param>
    /// <param name="logger">The logger.</param>
    public QueueListener(
        string queueName,
        IQueueProvider provider,
        ILogger logger)
    {
        QueueName = queueName;
        _provider = provider;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_isStarted)
        {
            return Task.CompletedTask;
        }

        _logger.LogDebug("Starting listener for queue {QueueName}", QueueName);

        _subscription = _provider.Subscribe<TPayload>(QueueName, (message, token) =>
        {
            _subject.OnNext(message);
            return Task.CompletedTask;
        }, _cts.Token);

        _isStarted = true;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (!_isStarted)
        {
            return Task.CompletedTask;
        }

        _logger.LogDebug("Stopping listener for queue {QueueName}", QueueName);

        _cts.Cancel();
        _subscription?.Dispose();
        _subscription = null;
        _isStarted = false;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public IDisposable Subscribe(Func<QueueMessage<TPayload>, CancellationToken, Task> handler, CancellationToken cancellationToken = default)
    {
        if (!_isStarted)
        {
            StartAsync(cancellationToken).GetAwaiter().GetResult();
        }

        var messageSubscription = Messages.Subscribe(async message =>
        {
            try
            {
                await handler(message, cancellationToken);
                await AcknowledgeAsync(message.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling message {MessageId} from queue {QueueName}",
                    message.Id, QueueName);

                await RejectAsync(message.Id, true, cancellationToken);
            }
        });

        return messageSubscription;
    }

    /// <inheritdoc />
    public Task AcknowledgeAsync(string messageId, CancellationToken cancellationToken = default)
    {
        return _provider.AcknowledgeAsync(QueueName, messageId, cancellationToken);
    }

    /// <inheritdoc />
    public Task RejectAsync(string messageId, bool requeue = false, CancellationToken cancellationToken = default)
    {
        return _provider.RejectAsync(QueueName, messageId, requeue, cancellationToken);
    }
}
