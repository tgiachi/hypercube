using HyperCube.Entities.Core.Types;
using HyperCube.Server.Core.Interfaces.Configs;

namespace HyperCube.Entities.Core.Data.Config;

/// <summary>
/// Configuration for the database connection and behavior.
/// </summary>
public class DatabaseConfig : IHyperConfig
{
    /// <summary>
    /// Gets or sets the database provider to use.
    /// </summary>
    public DatabaseProviderType DatabaseProvider { get; set; } = DatabaseProviderType.Sqlite;

    /// <summary>
    /// Gets or sets the connection string for the database.
    /// </summary>
    /// <remarks>
    /// For SQLite, this is the database file path.
    /// For other providers, this is the full connection string.
    /// Environment variables in the format {VARIABLE_NAME} will be automatically expanded.
    /// </remarks>
    public string ConnectionString { get; set; } = "Data Source=hypercube.db";

    /// <summary>
    /// Gets or sets whether to automatically migrate the database on startup.
    /// </summary>
    public bool AutoMigrate { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to enable sensitive data logging in EF Core.
    /// Should be disabled in production environments.
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to enable detailed errors in EF Core.
    /// Should be disabled in production environments.
    /// </summary>
    public bool EnableDetailedErrors { get; set; } = false;

    /// <summary>
    /// Gets or sets the command timeout in seconds.
    /// </summary>
    public int CommandTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets the maximum retry count for transient errors.
    /// </summary>
    public int MaxRetryCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the maximum batch size for batch operations.
    /// </summary>
    public int MaxBatchSize { get; set; } = 100;
}
