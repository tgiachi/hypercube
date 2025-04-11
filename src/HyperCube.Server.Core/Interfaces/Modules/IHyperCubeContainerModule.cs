using Microsoft.Extensions.DependencyInjection;

namespace HyperCube.Server.Core.Interfaces.Modules;

/// <summary>
///  Interface for HyperCube container modules.
/// </summary>
public interface IHyperCubeContainerModule
{
    /// <summary>
    ///  Registers the services for the module.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    IServiceCollection RegisterServices(IServiceCollection services);
}
