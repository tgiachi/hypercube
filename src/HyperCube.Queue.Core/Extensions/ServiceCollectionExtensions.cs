
using HyperCube.Queue.Core.Data.Config;

using HyperCube.Queue.Core.Interfaces;
using HyperCube.Queue.Core.Interfaces.Providers;
using HyperCube.Queue.Core.Interfaces.Services;
using HyperCube.Queue.Core.Providers;
using HyperCube.Queue.Core.Services;
using HyperCube.Queue.Core.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HyperCube.Queue.Core.Extensions;

/// <summary>
/// Extension methods for configuring queue services in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds HyperCube Queue services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddHyperCubeQueue(this IServiceCollection services)
    {
        return services.AddHyperCubeQueue(options => { });
    }

    /// <summary>
    /// Adds HyperCube Queue services to the specified <see cref="IServiceCollection" /> with custom configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configureOptions">A callback to configure the <see cref="QueueConfig"/>.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddHyperCubeQueue(
        this IServiceCollection services,
        Action<QueueConfig> configureOptions)
    {
        // Configure options
        var config = new QueueConfig();
        configureOptions(config);

        services.AddSingleton(config);
        services.Configure<QueueConfig>(options =>
        {
            options.ProviderType = config.ProviderType;
            options.ConnectionString = config.ConnectionString;
            options.OperationTimeoutMs = config.OperationTimeoutMs;
            options.MaxRetries = config.MaxRetries;
            options.RetryDelayMs = config.RetryDelayMs;
            options.AutoCreateQueues = config.AutoCreateQueues;
            options.QueuePrefix = config.QueuePrefix;
            options.MaxConcurrentConsumers = config.MaxConcurrentConsumers;
            options.InMemoryQueueBufferSize = config.InMemoryQueueBufferSize;
        });

        // Register provider
        switch (config.ProviderType)
        {
            case QueueProviderType.InMemory:
                services.TryAddSingleton<IQueueProvider, InMemoryQueueProvider>();
                break;

            // Other provider types would be registered here, but they are not
            // included in the core package and need separate packages

            default:
                services.TryAddSingleton<IQueueProvider, InMemoryQueueProvider>();
                break;
        }

        // Register queue service
        services.TryAddSingleton<IQueueService, QueueService>();

        return services;
    }

    /// <summary>
    /// Initializes the queue provider. This should be called after the service provider has been built.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>The same service provider so that multiple calls can be chained.</returns>
    public static IServiceProvider UseHyperCubeQueue(this IServiceProvider serviceProvider)
    {
        var provider = serviceProvider.GetRequiredService<IQueueProvider>();
        provider.InitializeAsync().GetAwaiter().GetResult();
        return serviceProvider;
    }
}
