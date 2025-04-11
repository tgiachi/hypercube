using CommandLine;
using HyperCube.Server.Core.Data.Options.Base;

namespace HyperCube.Server.Core.Extensions;

/// <summary>
/// Extension methods for parsing command line arguments into option objects.
/// </summary>
public static class ParseOptionCommandLineExtension
{
    /// <summary>
    /// Parses command line arguments into a specified options type.
    /// </summary>
    /// <typeparam name="TOption">The type of options to parse into, must inherit from BaseServerOptions.</typeparam>
    /// <param name="args">The command line arguments to parse.</param>
    /// <returns>The parsed options object.</returns>
    /// <remarks>
    /// This method:
    /// 1. Uses CommandLineParser to parse the arguments into the specified type
    /// 2. If any parsing errors occur, it displays them to the console and exits the application
    /// 3. Otherwise, it returns the parsed options object
    ///
    /// Note that this method will terminate the application with exit code -1 if parsing fails.
    ///
    /// Example usage:
    /// <code>
    /// var options = args.ParseOptionCommandLine&lt;MyServerOptions&gt;();
    /// Console.WriteLine($"Port: {options.Port}");
    /// </code>
    /// </remarks>
    public static TOption ParseOptionCommandLine<TOption>(this string[] args)
        where TOption : BaseServerOptions
    {
        // Parse the command line arguments into the specified options type
        var parsed = Parser.Default.ParseArguments<TOption>(args);

        // Check if there were any parsing errors
        if (parsed.Errors.Any())
        {
            // Display each error to the console
            foreach (var error in parsed.Errors)
            {
                Console.WriteLine($"Error: {error}");
            }

            // Exit the application with a non-zero status code to indicate failure
            Environment.Exit(-1);
        }

        // Get the successfully parsed options object
        var option = parsed.Value;

        return option;
    }
}
