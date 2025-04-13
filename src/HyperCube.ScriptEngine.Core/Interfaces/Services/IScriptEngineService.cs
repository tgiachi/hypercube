using HyperCube.Server.Core.Interfaces.Services.Base;

namespace HyperCube.ScriptEngine.Core.Interfaces.Services;

public interface IScriptEngineService : IHyperLoadableService
{

    void ExecuteScript(string script);

    void ExecuteScriptFile(string scriptFile);
    void AddCallback(string name, Action<object[]> callback);

    void AddConstant(string name, object value);
}
