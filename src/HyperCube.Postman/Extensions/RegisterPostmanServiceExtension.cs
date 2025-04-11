using HyperCube.Postman.Config;
using HyperCube.Postman.Interfaces.Services;
using HyperCube.Postman.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HyperCube.Postman.Extensions;

public static class RegisterPostmanServiceExtension
{
    /// <summary>
    /// Registers the Postman service with the specified service collection.
    /// </summary>
    /// <param name="services">The service collection to register the Postman service with.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddPostman(this IServiceCollection services, HyperPostmanConfig config)
    {
        services.AddSingleton<IHyperPostmanService, HyperPostmanService>();

        services.AddSingleton(config);

        return services;
    }

    public static IServiceCollection AddPostman(this IServiceCollection services, Action<HyperPostmanConfig> action)
    {
        var config = new HyperPostmanConfig();
        action(config);

        services.AddSingleton<IHyperPostmanService, HyperPostmanService>();
        services.AddSingleton(config);

        return services;
    }
}
