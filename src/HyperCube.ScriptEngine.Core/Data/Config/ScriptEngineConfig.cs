using HyperCube.Server.Core.Data.Events.Server;

namespace HyperCube.ScriptEngine.Core.Data.Config;

public class ScriptEngineConfig
{
    public List<string> BootstrapScripts { get; set; } = ["bootstrap.js", "index.js", "main.js"];

    public string ScriptDirectory { get; set; } = "scripts";


    public void AddBootstrapScript(string script)
    {
        if (!BootstrapScripts.Contains(script))
        {
            BootstrapScripts.Add(script);
        }
    }
}
