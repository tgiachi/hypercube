using HyperCube.Server.Core.Interfaces.Services;
using HyperCube.Server.Core.Interfaces.Services.Base;
using HyperCube.Server.Core.Internal;
using HyperCube.Server.Core.Types;
using Microsoft.Extensions.DependencyInjection;

namespace HyperCube.Server.Core.Extensions;

/// <summary>
///  Extension methods for adding HyperCube services.
/// </summary>
public static class AddHyperCubeServiceExtension
{
    public static IServiceCollection AddService(
        this IServiceCollection services, Type serviceType, Type implementationType,
        ServiceLifetimeType lifetimeType = ServiceLifetimeType.Singleton
    )
    {
        var lifetime = lifetimeType switch
        {
            ServiceLifetimeType.Singleton => ServiceLifetime.Singleton,
            ServiceLifetimeType.Scoped    => ServiceLifetime.Scoped,
            ServiceLifetimeType.Transient => ServiceLifetime.Transient,
            _                             => throw new ArgumentOutOfRangeException(nameof(lifetimeType), lifetimeType, null)
        };

        bool isAutoStart = typeof(IHyperAutoLoadService).IsAssignableFrom(implementationType);

        services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));

        services.AddToRegisterTypedList(
            new ServiceDefinitionObject(
                serviceType,
                implementationType,
                lifetimeType,
                isAutoStart
            )
        );

        return services;
    }

    public static IServiceCollection AddService<TService, TImplementation>(
        this IServiceCollection services,
        ServiceLifetimeType lifetimeType = ServiceLifetimeType.Singleton
    )
        where TService : class
        where TImplementation : class, TService
    {
        return services.AddService(typeof(TService), typeof(TImplementation), lifetimeType);
    }
}
