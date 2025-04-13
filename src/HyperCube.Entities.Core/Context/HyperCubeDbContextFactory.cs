using HyperCube.Core.Extensions;
using HyperCube.Entities.Core.Data.Config;
using HyperCube.Entities.Core.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HyperCube.Entities.Core.Context;

/// <summary>
/// Factory for creating database contexts with the appropriate provider and configuration.
/// </summary>
public class HyperCubeDbContextFactory
{
    private readonly DatabaseConfig _config;
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="HyperCubeDbContextFactory"/> class.
    /// </summary>
    /// <param name="config">The database configuration.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public HyperCubeDbContextFactory(
        DatabaseConfig config,
        ILoggerFactory loggerFactory
    )
    {
        _config = config;
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Creates a new database context with the configured provider.
    /// </summary>
    /// <returns>A configured database context.</returns>
    public HyperCubeDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<HyperCubeDbContext>();
        var logger = _loggerFactory.CreateLogger<HyperCubeDbContext>();

        ConfigureProvider(optionsBuilder);

        return new HyperCubeDbContext(optionsBuilder.Options, logger, _config);
    }

    /// <summary>
    /// Creates a new database context with the configured provider.
    /// </summary>
    /// <typeparam name="TContext">The type of context to create, must inherit from HyperCubeDbContext.</typeparam>
    /// <returns>A configured database context of the specified type.</returns>
    public TContext CreateDbContext<TContext>() where TContext : HyperCubeDbContext
    {
        var optionsBuilder = new DbContextOptionsBuilder<TContext>();
        var logger = _loggerFactory.CreateLogger<TContext>();

        ConfigureProvider(optionsBuilder);

        return (TContext)Activator.CreateInstance(
            typeof(TContext),
            optionsBuilder.Options,
            logger,
            _config
        )!;
    }

    private void ConfigureProvider(DbContextOptionsBuilder optionsBuilder)
    {
        // Process connection string to replace environment variables
        var connectionString = _config.ConnectionString.ReplaceEnvVariable();

        switch (_config.DatabaseProvider)
        {
            case DatabaseProviderType.Sqlite:
                optionsBuilder.UseSqlite(
                    connectionString,
                    options =>
                    {
                        options.MigrationsAssembly(typeof(HyperCubeDbContext).Assembly.FullName);
                        options.MaxBatchSize(_config.MaxBatchSize);
                    }
                );
                break;

            case DatabaseProviderType.SqlServer:
                optionsBuilder.UseSqlServer(
                    connectionString,
                    options =>
                    {
                        options.MigrationsAssembly(typeof(HyperCubeDbContext).Assembly.FullName);
                        options.EnableRetryOnFailure(_config.MaxRetryCount);
                        options.MaxBatchSize(_config.MaxBatchSize);
                        options.EnableRetryOnFailure(_config.MaxRetryCount);
                    }
                );
                break;

            case DatabaseProviderType.PostgreSql:
                optionsBuilder.UseNpgsql(
                    connectionString,
                    options =>
                    {
                        options.MigrationsAssembly(typeof(HyperCubeDbContext).Assembly.FullName);
                        options.CommandTimeout(_config.CommandTimeoutSeconds);
                        options.MaxBatchSize(_config.MaxBatchSize);
                        options.EnableRetryOnFailure(_config.MaxRetryCount);
                    }
                );
                break;

            case DatabaseProviderType.MySql:
                optionsBuilder.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    options =>
                    {
                        options.MigrationsAssembly(typeof(HyperCubeDbContext).Assembly.FullName);
                        options.CommandTimeout(_config.CommandTimeoutSeconds);
                        options.EnableRetryOnFailure(_config.MaxRetryCount);
                    }
                );
                break;

            case DatabaseProviderType.InMemory:
                optionsBuilder.UseInMemoryDatabase("HyperCubeInMemory");
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
