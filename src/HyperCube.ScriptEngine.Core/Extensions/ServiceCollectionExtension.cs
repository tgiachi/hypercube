using HyperCube.ScriptEngine.Core.Interfaces.Services;
using HyperCube.ScriptEngine.Core.ScriptModules;
using HyperCube.ScriptEngine.Core.Services;
using HyperCube.Server.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace HyperCube.ScriptEngine.Core.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddScriptEngine(this IServiceCollection services)
    {
        services
            .AddService<IScriptEngineService, ScriptEngineService>()
            .AddService<IEventDispatcherService, EventDispatcherService>();


        services
            .AddScriptModule<EventsModule>()
            .AddScriptModule<JsLoggerModule>();


        return services;
    }
}
