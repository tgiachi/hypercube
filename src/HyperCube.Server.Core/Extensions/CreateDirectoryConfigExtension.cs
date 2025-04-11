using HyperCube.Server.Core.Data.Directories.Base;
using HyperCube.Server.Core.Data.Options.Base;

namespace HyperCube.Server.Core.Extensions;

/// <summary>
/// Extension methods for creating directory configurations from server options.
/// </summary>
public static class CreateDirectoryConfigExtension
{
    /// <summary>
    /// Creates a directory configuration from server options.
    /// </summary>
    /// <typeparam name="TDirEnum">The enum type that defines the directory structure.</typeparam>
    /// <typeparam name="TOption">The type of server options, must inherit from BaseServerOptions.</typeparam>
    /// <param name="option">The server options containing the root directory path.</param>
    /// <returns>A new directory configuration initialized with the root directory from the options.</returns>
    /// <remarks>
    /// This method creates a BaseDirectoriesConfig using the RootDirectory specified in the options.
    /// The BaseDirectoriesConfig will automatically create all necessary directories based on the
    /// enum values defined in TDirEnum during initialization.
    ///
    /// Example usage:
    /// <code>
    /// var options = args.ParseOptionCommandLine&lt;MyServerOptions&gt;();
    /// var dirConfig = options.CreateDirectoryConfig&lt;DirectoryType, MyServerOptions&gt;();
    /// var logsPath = dirConfig[DirectoryType.Logs];
    /// </code>
    /// </remarks>
    public static BaseDirectoriesConfig<TDirEnum> CreateDirectoryConfig<TDirEnum, TOption>(this TOption option)
        where TDirEnum : struct, Enum
        where TOption : BaseServerOptions
    {
        // Create and return a new directory configuration using the root directory from the options
        return new BaseDirectoriesConfig<TDirEnum>(option.RootDirectory);
    }
}
