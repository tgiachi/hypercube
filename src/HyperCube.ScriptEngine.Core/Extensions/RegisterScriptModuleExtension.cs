using System.Diagnostics.CodeAnalysis;
using HyperCube.ScriptEngine.Core.Data.Internal;
using HyperCube.Server.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace HyperCube.ScriptEngine.Core.Extensions;

public static class RegisterScriptModuleExtension
{
    public static IServiceCollection AddScriptModule(this IServiceCollection services, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type moduleType)
    {
        services.AddSingleton(moduleType);
        services.AddToRegisterTypedList(new ScriptModuleData(moduleType));
        return services;
    }

    public static IServiceCollection AddScriptModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TModule>(this IServiceCollection services)
    {
        return services.AddScriptModule(typeof(TModule));
    }
}
