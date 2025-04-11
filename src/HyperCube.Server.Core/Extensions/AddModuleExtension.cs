using HyperCube.Server.Core.Interfaces.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace HyperCube.Server.Core.Extensions;

/// <summary>
///  Extension methods for adding modules to the service collection.
/// </summary>
public static class AddModuleExtension
{
    /// <summary>
    /// Extension method to add a module to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="moduleType"></param>
    /// <returns></returns>
    public static IServiceCollection AddModule(this IServiceCollection services, Type moduleType)
    {
        if (!typeof(IHyperCubeContainerModule).IsAssignableFrom(moduleType))
        {
            throw new ArgumentException($"Type {moduleType.Name} does not implement IHyperCubeContainerModule.");
        }

        var module = (IHyperCubeContainerModule)Activator.CreateInstance(moduleType);

        if (module == null)
        {
            throw new InvalidOperationException($"Could not create instance of {moduleType.Name}.");
        }

        module.RegisterServices(services);


        return services;
    }

    /// <summary>
    ///  Extension method to add a module to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="TModule"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddModule<TModule>(this IServiceCollection services)
        where TModule : IHyperCubeContainerModule
    {
        return services.AddModule(typeof(TModule));
    }
}
