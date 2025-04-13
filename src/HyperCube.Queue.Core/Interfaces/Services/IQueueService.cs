using HyperCube.Queue.Core.Interfaces.Listeners;
using HyperCube.Queue.Core.Messages;

namespace HyperCube.Queue.Core.Interfaces.Services;

/// <summary>
/// Interface for queue service that manages message publishers and listeners.
/// </summary>
public interface IQueueService
{
    /// <summary>
    /// Creates a publisher for the specified message type and queue.
    /// </summary>
    /// <typeparam name="TPayload">The type of the message payload.</typeparam>
    /// <param name="queueName">The name of the queue. If null, uses the type name.</param>
    /// <returns>A publisher for the specified message type.</returns>
    IQueuePublisher<TPayload> CreatePublisher<TPayload>(string? queueName = null) where TPayload : class;

    /// <summary>
    /// Creates a listener for the specified message type and queue.
    /// </summary>
    /// <typeparam name="TPayload">The type of the message payload.</typeparam>
    /// <param name="queueName">The name of the queue. If null, uses the type name.</param>
    /// <returns>A listener for the specified message type.</returns>
    IQueueListener<TPayload> CreateListener<TPayload>(string? queueName = null) where TPayload : class;

    /// <summary>
    /// Publishes a message to the specified queue.
    /// </summary>
    /// <typeparam name="TPayload">The type of the message payload.</typeparam>
    /// <param name="message">The message to publish.</param>
    /// <param name="queueName">The name of the queue. If null, uses the type name.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishAsync<TPayload>(QueueMessage<TPayload> message, string? queueName = null, CancellationToken cancellationToken = default)
        where TPayload : class;

    /// <summary>
    /// Publishes a payload to the specified queue.
    /// </summary>
    /// <typeparam name="TPayload">The type of the message payload.</typeparam>
    /// <param name="payload">The payload to publish.</param>
    /// <param name="queueName">The name of the queue. If null, uses the type name.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishAsync<TPayload>(TPayload payload, string? queueName = null, CancellationToken cancellationToken = default)
        where TPayload : class;

    /// <summary>
    /// Subscribes to a queue with the specified handler.
    /// </summary>
    /// <typeparam name="TPayload">The type of the message payload.</typeparam>
    /// <param name="handler">The handler to process messages.</param>
    /// <param name="queueName">The name of the queue. If null, uses the type name.</param>
    /// <param name="cancellationToken">A token to cancel the subscription.</param>
    /// <returns>A disposable subscription that can be used to unsubscribe.</returns>
    IDisposable Subscribe<TPayload>(Func<QueueMessage<TPayload>, CancellationToken, Task> handler, string? queueName = null, CancellationToken cancellationToken = default)
        where TPayload : class;
}
