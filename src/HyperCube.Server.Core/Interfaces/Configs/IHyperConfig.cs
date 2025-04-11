namespace HyperCube.Server.Core.Interfaces.Configs;

/// <summary>
/// Base marker interface for all configuration objects in the HyperCube framework.
/// </summary>
/// <remarks>
/// Classes implementing this interface represent configuration sections that can be
/// automatically discovered, loaded, and bound from various configuration sources
/// (e.g., appsettings.json, environment variables, command line arguments).
///
/// The HyperCube framework will automatically register implementations of this interface
/// with the dependency injection container, making them available for injection into services.
///
/// Example usage:
/// <code>
/// public class DatabaseConfig : IHyperConfig
/// {
///     public string ConnectionString { get; set; } = string.Empty;
///     public int CommandTimeout { get; set; } = 30;
///     public bool EnablePooling { get; set; } = true;
/// }
/// </code>
///
/// Then inject and use in a service:
/// <code>
/// public class DataService
/// {
///     private readonly DatabaseConfig _config;
///
///     public DataService(DatabaseConfig config)
///     {
///         _config = config;
///     }
/// }
/// </code>
/// </remarks>
public interface IHyperConfig
{
    // This is a marker interface with no members
}
