using HyperCube.Queue.Core.Types;
using HyperCube.Server.Core.Interfaces.Configs;

namespace HyperCube.Queue.Core.Data.Config;

/// <summary>
/// Configuration for the queue system.
/// </summary>
public class QueueConfig : IHyperConfig
{
    /// <summary>
    /// Gets or sets the type of queue provider to use.
    /// </summary>
    public QueueProviderType ProviderType { get; set; } = QueueProviderType.InMemory;

    /// <summary>
    /// Gets or sets the connection string for the queue provider.
    /// Not used for InMemory provider.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default timeout for queue operations in milliseconds.
    /// </summary>
    public int OperationTimeoutMs { get; set; } = 30000;

    /// <summary>
    /// Gets or sets the maximum number of retries for queue operations.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Gets or sets the retry delay in milliseconds.
    /// </summary>
    public int RetryDelayMs { get; set; } = 1000;

    /// <summary>
    /// Gets or sets whether to auto-create queues that don't exist.
    /// </summary>
    public bool AutoCreateQueues { get; set; } = true;

    /// <summary>
    /// Gets or sets the default queue prefix.
    /// </summary>
    public string QueuePrefix { get; set; } = "hypercube.";

    /// <summary>
    /// Gets or sets the maximum number of concurrent consumers per queue.
    /// </summary>
    public int MaxConcurrentConsumers { get; set; } = 8;

    /// <summary>
    /// Gets or sets the size of the in-memory queue buffer.
    /// Only applicable for InMemory provider.
    /// </summary>
    public int InMemoryQueueBufferSize { get; set; } = 10000;
}


