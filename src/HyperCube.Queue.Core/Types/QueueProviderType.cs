namespace HyperCube.Queue.Core.Types;

/// <summary>
/// Supported queue provider types.
/// </summary>
public enum QueueProviderType
{
    /// <summary>
    /// In-memory queue provider for testing and simple applications.
    /// </summary>
    InMemory,

    /// <summary>
    /// RabbitMQ queue provider.
    /// Requires HyperCube.Queue.RabbitMq package.
    /// </summary>
    RabbitMq,

    /// <summary>
    /// AWS SQS queue provider.
    /// Requires HyperCube.Queue.Sqs package.
    /// </summary>
    AwsSqs,

    /// <summary>
    /// Azure Service Bus queue provider.
    /// Requires HyperCube.Queue.AzureServiceBus package.
    /// </summary>
    AzureServiceBus,

    /// <summary>
    /// Kafka queue provider.
    /// Requires HyperCube.Queue.Kafka package.
    /// </summary>
    Kafka
}
