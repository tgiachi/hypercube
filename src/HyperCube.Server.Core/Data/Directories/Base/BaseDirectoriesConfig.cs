using HyperCube.Core.Extensions;

namespace HyperCube.Server.Core.Data.Directories.Base;

/// <summary>
/// Manages application directories in a type-safe and extensible way.
/// </summary>
/// <remarks>
/// This class provides a flexible way to handle directory structures for different applications.
/// It works with any enum type, allowing specific applications to define their own directory structures.
///
/// Example of customizing with your own enum:
/// <code>
/// // Custom directory structure for a specific application
/// public enum MyAppDirectories
/// {
///     Config,
///     Data,
///     Cache,
///     Plugins
/// }
///
/// // Creating a custom directory config
/// var dirs = new DirectoriesConfig&lt;MyAppDirectories&gt;("/path/to/root");
///
/// // Getting paths
/// string configPath = dirs[MyAppDirectories.Config];
/// string dataPath = dirs.GetPath(MyAppDirectories.Data);
///
/// // You can also override naming conventions
/// var customDirs = new DirectoriesConfig&lt;MyAppDirectories&gt;("/path/to/root",
///     dir => dir.ToString().ToLowerInvariant());
/// </code>
/// </remarks>
public class BaseDirectoriesConfig<TDirectoryEnum> where TDirectoryEnum : struct, Enum
{
    private readonly Dictionary<TDirectoryEnum, string> _directoryPaths = new();
    private readonly Func<TDirectoryEnum, string> _nameConverter;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseDirectoriesConfig{TDirectoryEnum}"/> class.
    /// </summary>
    /// <param name="rootDirectory">The root directory path.</param>
    /// <param name="nameConverter">Optional function to convert enum values to directory names. If not provided, uses snake_case by default.</param>
    public BaseDirectoriesConfig(string rootDirectory, Func<TDirectoryEnum, string>? nameConverter = null)
    {
        Root = rootDirectory;
        _nameConverter = nameConverter ?? (dir => dir.ToString().ToSnakeCase());

        Init();
    }

    /// <summary>
    /// Gets the root directory path.
    /// </summary>
    public string Root { get; }

    /// <summary>
    /// Gets the path for the specified directory type.
    /// </summary>
    /// <param name="directoryType">The directory type.</param>
    /// <returns>The full path for the specified directory type.</returns>
    public string this[TDirectoryEnum directoryType] => GetPath(directoryType);

    /// <summary>
    /// Gets the full path for the specified directory type.
    /// </summary>
    /// <param name="directoryType">The directory type.</param>
    /// <returns>The full path for the specified directory type.</returns>
    public string GetPath(TDirectoryEnum directoryType)
    {
        // If we've already computed this path, return it from cache
        if (_directoryPaths.TryGetValue(directoryType, out var path))
        {
            return path;
        }

        // Root is a special case, just return the root directory
        if (EqualityComparer<TDirectoryEnum>.Default.Equals(
                directoryType,
                Enum.Parse<TDirectoryEnum>("Root", true)
            ))
        {
            return Root;
        }

        // For other directories, combine root with the converted name
        path = Path.Combine(Root, _nameConverter(directoryType));
        _directoryPaths[directoryType] = path;
        return path;
    }

    /// <summary>
    /// Returns the string representation of this instance, which is the root directory path.
    /// </summary>
    /// <returns>The root directory path.</returns>
    public override string ToString()
    {
        return Root;
    }

    private void Init()
    {
        // Create root directory if it doesn't exist
        if (!Directory.Exists(Root))
        {
            Directory.CreateDirectory(Root);
        }

        // Get all enum values except "Root"
        const string rootName = "Root";
        var directoryTypes = Enum.GetValues<TDirectoryEnum>()
            .Where(d => !d.ToString().Equals(rootName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Create all subdirectories
        foreach (var directory in directoryTypes)
        {
            var path = GetPath(directory);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
