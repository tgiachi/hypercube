using System.Reflection;
using HyperCube.Core.Extensions;
using HyperCube.Postman.Interfaces.Services;
using HyperCube.ScriptEngine.Core.Attributes.Scripts;
using HyperCube.ScriptEngine.Core.Data.Config;
using HyperCube.ScriptEngine.Core.Data.Internal;
using HyperCube.ScriptEngine.Core.Interfaces.Services;
using HyperCube.ScriptEngine.Core.Utils;
using HyperCube.Server.Core.Data.Events.Server;
using Jint;
using Jint.Runtime.Interop;
using Microsoft.Extensions.Logging;

namespace HyperCube.ScriptEngine.Core.Services;

public class ScriptEngineService : IScriptEngineService, ILetterListener<ServerReadyEvent>
{
    private readonly ILogger _logger;

    private readonly List<string> _initScripts;

    private readonly ScriptEngineConfig _scriptEngineConfig;
    private readonly Engine _jsEngine;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<ScriptModuleData> _scriptModules;

    private readonly Dictionary<string, Action<object[]>> _callbacks = new();
    private readonly Dictionary<string, object> _constants = new();


    public ScriptEngineService(
        ILogger<ScriptEngineService> logger, IServiceProvider serviceProvider,
        List<ScriptModuleData> scriptModules, IHyperPostmanService hyperPostmanService, ScriptEngineConfig scriptEngineConfig
    )
    {
        _logger = logger;
        _initScripts = scriptEngineConfig.BootstrapScripts;

        _serviceProvider = serviceProvider;
        _scriptModules = scriptModules;


        _scriptEngineConfig = scriptEngineConfig;

        var typeResolver = TypeResolver.Default;

        typeResolver.MemberNameCreator = MemberNameCreator;
        _jsEngine = new Engine(
            options =>
            {
                options.EnableModules(_scriptEngineConfig.ScriptDirectory);
                options.AllowClr(GetType().Assembly);
                options.SetTypeResolver(typeResolver);
            }
        );

        hyperPostmanService.Subscribe(this);
    }

    private IEnumerable<string> MemberNameCreator(MemberInfo memberInfo)
    {
        _logger.LogTrace("[JS] Creating member name  {MemberInfo}", memberInfo.Name.ToSnakeCase());
        yield return memberInfo.Name.ToSnakeCase();
    }


    public Task HandleAsync(ServerReadyEvent signalEvent, CancellationToken cancellationToken = default)

    {
        if (_callbacks.TryGetValue("onStarted", out var callback))
        {
            _logger.LogInformation("Executing onStarted");
            callback(null);
        }

        return Task.CompletedTask;
    }

    private void ExecuteBootstrap()
    {
        foreach (var file in _initScripts.Select(s => Path.Combine(_scriptEngineConfig.ScriptDirectory, s)))
        {
            if (File.Exists(file))
            {
                var fileName = Path.GetFileName(file);
                _logger.LogInformation("Executing {FileName}  script", fileName);
                ExecuteScriptFile(file);
            }
        }
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        AddConstant("Version", "1.0.0");

        foreach (var module in _scriptModules)
        {
            var scriptModuleAttribute = module.ModuleType.GetCustomAttribute<ScriptModuleAttribute>();
            var instance = _serviceProvider.GetService(module.ModuleType);

            if (instance == null)
            {
                throw new InvalidOperationException(
                    $"Unable to create instance of script module {module.ModuleType.Name}"
                );
            }

            _logger.LogDebug("Registering script module {Name}", scriptModuleAttribute.Name);

            _jsEngine.SetValue(scriptModuleAttribute.Name, instance);
        }


        _logger.LogDebug("Generating scripts documentation in scripts directory named 'index.d.ts'");
        var documentation = TypeScriptDocumentationGenerator.GenerateDocumentation(_scriptModules, _constants);

        File.WriteAllText(Path.Combine(_scriptEngineConfig.ScriptDirectory, "index.d.ts"), documentation);


        ExecuteBootstrap();


        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void ExecuteScript(string script)
    {
        try
        {
            _jsEngine.Execute(script);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error executing script");
        }
    }

    public void ExecuteScriptFile(string scriptFile)
    {
        var content = File.ReadAllText(scriptFile);

        ExecuteScript(content);
    }

    public void AddCallback(string name, Action<object[]> callback)
    {
        _callbacks[name] = callback;
    }

    public void AddConstant(string name, object value)
    {
        _constants[name.ToSnakeCaseUpper()] = value;
        _jsEngine.SetValue(name.ToSnakeCaseUpper(), value);
    }
}
