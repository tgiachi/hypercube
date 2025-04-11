using HyperCube.Core.Extensions;
using HyperCube.Server.Core.Data.Configs.Base;
using HyperCube.Server.Core.Data.Directories.Base;
using Microsoft.Extensions.DependencyInjection;


namespace HyperCube.Server.Core.Extensions;

/// <summary>
///  Extension methods for loading configuration files.
/// </summary>
public static class LoadConfigExtension
{
    /// <summary>
    /// Loads a configuration file from disk or creates a default one if it doesn't exist.
    /// </summary>
    /// <typeparam name="TConfig">The type of configuration to load, must inherit from BaseServerConfig.</typeparam>
    /// <typeparam name="TDirectoryEnum">The enum type used for directory structure.</typeparam>
    /// <param name="services">The service collection to add the configuration to.</param>
    /// <param name="directoriesConfig">The directories configuration that provides the root path.</param>
    /// <returns>The loaded configuration instance.</returns>
    /// <remarks>
    /// This method:
    /// 1. Determines the configuration filename by removing "Config" from the type name and converting to snake_case
    /// 2. Checks if the configuration file exists in the root directory
    /// 3. If it doesn't exist, creates a new configuration with default values and saves it to disk
    /// 4. Reads the configuration content from the file
    /// 5. Deserializes the YAML content to the configuration type
    /// 6. Registers the configuration instance as a singleton in the dependency injection container
    /// 7. Returns the loaded configuration instance for immediate use
    ///
    /// Example usage:
    /// <code>
    /// var webConfig = services.LoadConfig&lt;WebServerConfig, DirectoryType&gt;(directoriesConfig);
    /// Console.WriteLine($"Loaded configuration: Port={webConfig.Port}");
    /// </code>
    /// </remarks>
    public static TConfig LoadConfig<TConfig, TDirectoryEnum>(
        this IServiceCollection services, BaseDirectoriesConfig<TDirectoryEnum> directoriesConfig
    )
        where TConfig : BaseServerConfig, new()
        where TDirectoryEnum : struct, Enum
    {
        // Derive configuration filename from the type name (e.g., "WebServerConfig" becomes "web_server.yaml")
        var configFileName = typeof(TConfig).Name.Replace("Config", "").ToSnakeCase() + ".yaml";

        // Build the full path to the configuration file
        var fullConfigFilePath = Path.Combine(directoriesConfig.Root, configFileName);

        // Check if the configuration file exists
        if (!File.Exists(fullConfigFilePath))
        {
            // Create a default configuration with default values
            var config = new TConfig();

            // Save the default configuration to disk
            File.WriteAllText(fullConfigFilePath, config.ToYaml());
        }

        // Read the configuration content from the file
        var configContent = File.ReadAllText(fullConfigFilePath);

        // Deserialize the YAML content to the configuration type
        var configInstance = configContent.FromYaml<TConfig>();

        // Register the configuration instance in the DI container
        services.AddSingleton(configInstance);

        // Return the loaded configuration for immediate use
        return configInstance;
    }
}
