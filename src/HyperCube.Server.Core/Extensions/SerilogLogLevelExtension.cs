using HyperCube.Server.Core.Types;
using Serilog.Events;

namespace HyperCube.Server.Core.Extensions;

/// <summary>
/// Provides extension methods for converting between HyperCube log levels and Serilog log levels.
/// </summary>
public static class SerilogLogLevelExtension
{
    /// <summary>
    /// Converts a HyperCube <see cref="LogLevelType"/> to a Serilog <see cref="LogEventLevel"/>.
    /// </summary>
    /// <param name="type">The HyperCube log level to convert.</param>
    /// <returns>The equivalent Serilog log event level.</returns>
    /// <remarks>
    /// This extension method allows for seamless integration between HyperCube's logging abstraction
    /// and Serilog's implementation. The mapping follows standard logging level conventions:
    ///
    /// - LogLevelType.Trace maps to LogEventLevel.Verbose (Serilog doesn't have a "Trace" level)
    /// - LogLevelType.Debug maps to LogEventLevel.Debug
    /// - LogLevelType.Information maps to LogEventLevel.Information
    /// - LogLevelType.Warning maps to LogEventLevel.Warning
    /// - LogLevelType.Error maps to LogEventLevel.Error
    ///
    /// Any undefined log level will default to LogEventLevel.Information.
    /// </remarks>
    public static LogEventLevel ToLogEventLevel(this LogLevelType type) =>
        type switch
        {
            LogLevelType.Trace       => LogEventLevel.Verbose,
            LogLevelType.Debug       => LogEventLevel.Debug,
            LogLevelType.Information => LogEventLevel.Information,
            LogLevelType.Warning     => LogEventLevel.Warning,
            LogLevelType.Error       => LogEventLevel.Error,
            _                        => LogEventLevel.Information
        };
}
