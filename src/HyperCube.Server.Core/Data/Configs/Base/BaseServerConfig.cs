using HyperCube.Server.Core.Types;

namespace HyperCube.Server.Core.Data.Configs.Base;

/// <summary>
/// Base configuration class for all server configurations in the HyperCube framework.
/// </summary>
/// <remarks>
/// This class provides common configuration properties that all servers should have.
/// Specialized server configurations should inherit from this class and add their
/// specific configuration properties.
///
/// Example of inheritance:
/// <code>
/// public class WebApiServerConfig : BaseServerConfig, IHyperConfig
/// {
///     public string Host { get; set; } = "localhost";
///     public int Port { get; set; } = 8080;
///     public bool EnableSwagger { get; set; } = true;
///     public List&lt;string&gt; CorsOrigins { get; set; } = new() { "*" };
/// }
///
/// // Usage:
/// var config = new WebApiServerConfig
/// {
///     LogLevel = LogLevelType.Debug,
///     Port = 5000,
///     EnableSwagger = false
/// };
/// </code>
/// </remarks>
public class BaseServerConfig
{
    /// <summary>
    /// Gets or sets the logging level for the server.
    /// </summary>
    /// <remarks>
    /// Controls the verbosity of logs. The default is Information level,
    /// which provides a balance between useful information and log volume.
    /// </remarks>
    public LogLevelType LogLevel { get; set; } = LogLevelType.Information;
}
