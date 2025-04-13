using HyperCube.Queue.Core.Messages;

namespace HyperCube.Queue.Core.Interfaces.Listeners;

/// <summary>
/// Interface for listening to messages from a queue.
/// </summary>
/// <typeparam name="TPayload">The type of the message payload.</typeparam>
public interface IQueueListener<TPayload> where TPayload : class
{
    /// <summary>
    /// Gets the name of the queue.
    /// </summary>
    string QueueName { get; }

    /// <summary>
    /// Gets an observable sequence of messages from the queue.
    /// </summary>
    IObservable<QueueMessage<TPayload>> Messages { get; }

    /// <summary>
    /// Starts listening to the queue.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops listening to the queue.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribes to the queue with the specified handler.
    /// </summary>
    /// <param name="handler">The handler to process messages.</param>
    /// <param name="cancellationToken">A token to cancel the subscription.</param>
    /// <returns>A disposable subscription that can be used to unsubscribe.</returns>
    IDisposable Subscribe(Func<QueueMessage<TPayload>, CancellationToken, Task> handler, CancellationToken cancellationToken = default);

    /// <summary>
    /// Acknowledges that a message has been processed successfully.
    /// </summary>
    /// <param name="messageId">The ID of the message to acknowledge.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AcknowledgeAsync(string messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rejects a message, optionally requeuing it.
    /// </summary>
    /// <param name="messageId">The ID of the message to reject.</param>
    /// <param name="requeue">Whether to requeue the message.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RejectAsync(string messageId, bool requeue = false, CancellationToken cancellationToken = default);
}
