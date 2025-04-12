using System;
using System.IO;
using HyperCube.Server.Core.Data.Directories.Base;
using HyperCube.Server.Core.Data.Options.Base;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;

namespace HyperCube.Server.Core.Extensions;

/// <summary>
/// Extension methods for configuring and building loggers for HyperCube applications.
/// </summary>
public static class LoggerBuildExtension
{
    /// <summary>
    /// Configures and builds a Serilog logger for the application.
    /// </summary>
    /// <typeparam name="TDirectoriesEnum">The enum type that defines the directory structure.</typeparam>
    /// <typeparam name="TBasicServerOptions">The type of server options, must inherit from BaseServerOptions.</typeparam>
    /// <param name="hostBuilder">The host application builder to configure logging for.</param>
    /// <param name="directoriesConfig">The directory configuration containing the root directory.</param>
    /// <param name="serverOptions">The server options containing logging preferences.</param>
    /// <returns>The modified host application builder for chaining.</returns>
    /// <remarks>
    /// This method:
    /// 1. Clears any existing logging providers
    /// 2. Ensures the logs directory exists
    /// 3. Configures Serilog to write to both console and file outputs
    /// 4. Sets the minimum log level based on the server options
    /// 5. Adds the configured Serilog logger to the host's logging pipeline
    ///
    /// The log files are written in compact JSON format to facilitate log analysis.
    /// </remarks>
    public static IHostApplicationBuilder BuildLogger<TDirectoriesEnum, TBasicServerOptions>(
        this IHostApplicationBuilder hostBuilder, BaseDirectoriesConfig<TDirectoriesEnum> directoriesConfig,
        TBasicServerOptions serverOptions
    ) where TDirectoriesEnum : struct, Enum
        where TBasicServerOptions : BaseServerOptions
    {
        // Clear any existing logging providers to avoid duplication
        hostBuilder.Logging.ClearProviders();

        // Determine the logs directory path
        var logsDirectory = Path.Combine(directoriesConfig.Root, serverOptions.LogsDirectory);

        // Create the logs directory if it doesn't exist
        if (!Directory.Exists(logsDirectory))
        {
            Directory.CreateDirectory(logsDirectory);
        }

        // Create and configure the logger
        var loggingConfig = new LoggerConfiguration();

        loggingConfig = loggingConfig
            .WriteTo.Console() // Write logs to console
            .WriteTo.File(new CompactJsonFormatter(), Path.Combine(logsDirectory, "server_.log")) // Write logs to file in JSON format
            .MinimumLevel.Is(serverOptions.LogLevel.ToLogEventLevel()); // Set minimum log level from options

        // Create the Serilog logger
        Log.Logger = loggingConfig.CreateLogger();

        // Add Serilog to the host builder's logging pipeline
        hostBuilder.Logging.AddSerilog(Log.Logger);

        return hostBuilder;
    }
}
