using HyperCube.Queue.Core.Messages;

namespace HyperCube.Queue.Core.Interfaces.Providers;

/// <summary>
/// Interface for queue providers that handle the low-level queue operations.
/// </summary>
public interface IQueueProvider
{
    /// <summary>
    /// Gets the type of this queue provider.
    /// </summary>
    string ProviderType { get; }

    /// <summary>
    /// Initializes the queue provider.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InitializeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a queue with the specified name if it doesn't exist.
    /// </summary>
    /// <param name="queueName">The name of the queue to create.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateQueueAsync(string queueName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a queue with the specified name.
    /// </summary>
    /// <param name="queueName">The name of the queue to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteQueueAsync(string queueName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a queue with the specified name exists.
    /// </summary>
    /// <param name="queueName">The name of the queue to check.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that completes with a boolean indicating whether the queue exists.</returns>
    Task<bool> QueueExistsAsync(string queueName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of all queues.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that completes with a list of queue names.</returns>
    Task<IEnumerable<string>> GetQueuesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a message to the specified queue.
    /// </summary>
    /// <typeparam name="TPayload">The type of the message payload.</typeparam>
    /// <param name="queueName">The name of the queue.</param>
    /// <param name="message">The message to publish.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishAsync<TPayload>(string queueName, QueueMessage<TPayload> message, CancellationToken cancellationToken = default)
        where TPayload : class;

    /// <summary>
    /// Subscribes to the specified queue.
    /// </summary>
    /// <typeparam name="TPayload">The type of the message payload.</typeparam>
    /// <param name="queueName">The name of the queue.</param>
    /// <param name="handler">The handler to process messages.</param>
    /// <param name="cancellationToken">A token to cancel the subscription.</param>
    /// <returns>A disposable subscription that can be used to unsubscribe.</returns>
    IDisposable Subscribe<TPayload>(string queueName, Func<QueueMessage<TPayload>, CancellationToken, Task> handler, CancellationToken cancellationToken = default)
        where TPayload : class;

    /// <summary>
    /// Acknowledges that a message has been processed successfully.
    /// </summary>
    /// <param name="queueName">The name of the queue.</param>
    /// <param name="messageId">The ID of the message to acknowledge.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AcknowledgeAsync(string queueName, string messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rejects a message, optionally requeuing it.
    /// </summary>
    /// <param name="queueName">The name of the queue.</param>
    /// <param name="messageId">The ID of the message to reject.</param>
    /// <param name="requeue">Whether to requeue the message.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RejectAsync(string queueName, string messageId, bool requeue = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the number of messages in the specified queue.
    /// </summary>
    /// <param name="queueName">The name of the queue.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that completes with the message count.</returns>
    Task<int> GetMessageCountAsync(string queueName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Purges all messages from the specified queue.
    /// </summary>
    /// <param name="queueName">The name of the queue.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PurgeQueueAsync(string queueName, CancellationToken cancellationToken = default);
}
