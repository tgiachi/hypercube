namespace HyperCube.Entities.Core.Types;

/// <summary>
/// Supported database provider types.
/// </summary>
public enum DatabaseProviderType
{
    /// <summary>
    /// SQLite database provider.
    /// </summary>
    Sqlite,

    /// <summary>
    /// SQL Server database provider.
    /// </summary>
    SqlServer,

    /// <summary>
    /// PostgreSQL database provider.
    /// </summary>
    PostgreSql,

    /// <summary>
    /// MySQL database provider.
    /// </summary>
    MySql,

    /// <summary>
    /// In-memory database provider (for testing).
    /// </summary>
    InMemory
}
