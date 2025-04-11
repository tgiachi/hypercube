using System.Reflection;
using HyperCube.Core.Utils;
using HyperCube.Server.Core.Data.Options.Base;

namespace HyperCube.Server.Core.Extensions;

/// <summary>
/// Extension methods for displaying application headers.
/// </summary>
public static class ShowHeaderExtension
{
    /// <summary>
    /// Displays a text header from an embedded resource if the ShowHeader option is enabled.
    /// </summary>
    /// <param name="options">The server options containing the ShowHeader flag.</param>
    /// <param name="headerFile">The path to the embedded header resource file. Defaults to "Assets/header.txt".</param>
    /// <param name="assembly">The assembly containing the embedded resource. If null, uses the executing assembly.</param>
    /// <remarks>
    /// This method reads a header text file from embedded resources and displays it line by line
    /// in the console. It only displays the header if the ShowHeader flag in the options is true.
    ///
    /// The header file is typically used to display application name, version, and copyright
    /// information when the application starts.
    /// </remarks>
    public static void ShowHeader(
        this BaseServerOptions options, string headerFile = "Assets/header.txt", Assembly assembly = null
    )
    {
        if (options.ShowHeader)
        {
            assembly ??= Assembly.GetExecutingAssembly();
            var content = ResourceUtils.ReadEmbeddedResource(headerFile, assembly);

            foreach (var line in content.Split(Environment.NewLine))
            {
                Console.WriteLine(line);
            }
        }
    }
}
