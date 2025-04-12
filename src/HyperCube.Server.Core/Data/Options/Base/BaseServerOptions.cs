using CommandLine;
using HyperCube.Server.Core.Types;

namespace HyperCube.Server.Core.Data.Options.Base;

/// <summary>
/// Base options class for command-line arguments in HyperCube server applications.
/// </summary>
/// <remarks>
/// This class defines common command-line options that all server applications
/// should support. Specialized server applications should inherit from this class
/// and add their specific command-line options.
///
/// Example of inheritance:
/// <code>
/// public class WebApiServerOptions : BaseServerOptions
/// {
///     [Option('p', "port", Required = false, Default = 8080, HelpText = "The port to listen on.")]
///     public int Port { get; set; }
///
///     [Option('h', "host", Required = false, Default = "localhost", HelpText = "The host address to bind to.")]
///     public string Host { get; set; } = "localhost";
///
///     [Option("enable-cors", Required = false, Default = true, HelpText = "Enable CORS for the API.")]
///     public bool EnableCors { get; set; } = true;
/// }
///
/// // Usage with CommandLineParser:
/// Parser.Default.ParseArguments&lt;WebApiServerOptions&gt;(args)
///     .WithParsed(options =>
///     {
///         Console.WriteLine($"Starting server on {options.Host}:{options.Port}");
///         Console.WriteLine($"Log level: {options.LogLevel}");
///         Console.WriteLine($"Root directory: {options.RootDirectory}");
///     });
/// </code>
/// </remarks>
public class BaseServerOptions
{
    /// <summary>
    /// Gets or sets the logging level for the server.
    /// </summary>
    /// <remarks>
    /// Controls the verbosity of the logs. The default is Information level.
    /// </remarks>
    [Option(
        'l',
        "log-level",
        Required = false,
        Default = LogLevelType.Information,
        HelpText = "Set the logging level for the server."
    )]
    public LogLevelType LogLevel { get; set; }

    /// <summary>
    /// Gets or sets the root directory for the service.
    /// </summary>
    /// <remarks>
    /// Specifies the base directory where the application should look for files and resources.
    /// If empty, typically the current working directory is used.
    /// </remarks>
    [Option('r', "root-directory", Required = false, Default = "", HelpText = "Set the root directory for the service.")]
    public string RootDirectory { get; set; } = string.Empty;

    /// <summary>
    ///  Show header in the console.
    /// </summary>
    [Option('s', "show-header", Required = false, Default = true, HelpText = "Show header in the console.")]
    public bool ShowHeader { get; set; } = true;


    /// <summary>
    ///  /// Gets or sets the logs directory for the service.
    /// </summary>
    [Option('d', "logs-directory", Required = false, Default = "logs", HelpText = "Set the logs directory for the service.")]
    public string LogsDirectory { get; set; }
}
