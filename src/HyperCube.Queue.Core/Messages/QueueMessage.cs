using System.Text.Json.Serialization;

namespace HyperCube.Queue.Core.Messages;

/// <summary>
/// Base class for all queue messages.
/// </summary>
public abstract class QueueMessage
{
    /// <summary>
    /// Gets or sets the unique message ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the timestamp when this message was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the correlation ID for tracking related messages.
    /// </summary>
    [JsonPropertyName("correlation_id")]
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the retry count for this message.
    /// </summary>
    [JsonPropertyName("retry_count")]
    public int RetryCount { get; set; }

    /// <summary>
    /// Gets or sets the scheduled delivery time for delayed messages.
    /// </summary>
    [JsonPropertyName("scheduled_delivery")]
    public DateTime? ScheduledDelivery { get; set; }

    /// <summary>
    /// Gets or sets the metadata dictionary for arbitrary data.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// Generic queue message with typed payload.
/// </summary>
/// <typeparam name="TPayload">The type of the message payload.</typeparam>
public class QueueMessage<TPayload> : QueueMessage where TPayload : class
{
    /// <summary>
    /// Gets or sets the message payload.
    /// </summary>
    [JsonPropertyName("payload")]
    public TPayload Payload { get; set; } = default!;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueMessage{TPayload}"/> class.
    /// </summary>
    public QueueMessage()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueMessage{TPayload}"/> class with the specified payload.
    /// </summary>
    /// <param name="payload">The message payload.</param>
    public QueueMessage(TPayload payload)
    {
        Payload = payload;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueMessage{TPayload}"/> class with the specified payload and correlation ID.
    /// </summary>
    /// <param name="payload">The message payload.</param>
    /// <param name="correlationId">The correlation ID.</param>
    public QueueMessage(TPayload payload, string correlationId) : this(payload)
    {
        CorrelationId = correlationId;
    }
}
