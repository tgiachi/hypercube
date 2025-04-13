using HyperCube.Queue.Core.Messages;

namespace HyperCube.Queue.Core.Interfaces.Listeners;

/// <summary>
/// Interface for publishing messages to a queue.
/// </summary>
/// <typeparam name="TPayload">The type of the message payload.</typeparam>
public interface IQueuePublisher<TPayload> where TPayload : class
{
    /// <summary>
    /// Gets the name of the queue.
    /// </summary>
    string QueueName { get; }

    /// <summary>
    /// Publishes a message to the queue.
    /// </summary>
    /// <param name="message">The message to publish.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishAsync(QueueMessage<TPayload> message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a payload to the queue.
    /// </summary>
    /// <param name="payload">The payload to publish.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishAsync(TPayload payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// Schedules a message to be published at a later time.
    /// </summary>
    /// <param name="message">The message to publish.</param>
    /// <param name="delay">The delay before publishing.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ScheduleAsync(QueueMessage<TPayload> message, TimeSpan delay, CancellationToken cancellationToken = default);

    /// <summary>
    /// Schedules a payload to be published at a later time.
    /// </summary>
    /// <param name="payload">The payload to publish.</param>
    /// <param name="delay">The delay before publishing.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ScheduleAsync(TPayload payload, TimeSpan delay, CancellationToken cancellationToken = default);

    /// <summary>
    /// Schedules a message to be published at a specific time.
    /// </summary>
    /// <param name="message">The message to publish.</param>
    /// <param name="scheduledTime">The time to publish the message.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ScheduleAsync(QueueMessage<TPayload> message, DateTime scheduledTime, CancellationToken cancellationToken = default);

    /// <summary>
    /// Schedules a payload to be published at a specific time.
    /// </summary>
    /// <param name="payload">The payload to publish.</param>
    /// <param name="scheduledTime">The time to publish the message.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ScheduleAsync(TPayload payload, DateTime scheduledTime, CancellationToken cancellationToken = default);
}
