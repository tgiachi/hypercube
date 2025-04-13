using HyperCube.Core.Extensions;

using HyperCube.Entities.Core.Context;
using HyperCube.Entities.Core.Data.Config;
using HyperCube.Entities.Core.Entities.Base;

using HyperCube.Entities.Core.Interfaces.Repositories;

using HyperCube.Entities.Core.Repositories;
using HyperCube.Entities.Core.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HyperCube.Entities.Core.Extensions;

/// <summary>
/// Extension methods for setting up Entity Framework services in an <see cref="IServiceCollection" />.
/// </summary>
public static class EntityFrameworkServiceExtensions
{
    /// <summary>
    /// Adds Entity Framework Core services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="config">The database configuration.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddHyperCubeEntityFramework(
        this IServiceCollection services,
        DatabaseConfig config)
    {
        // Register database configuration
        services.AddSingleton(config);

        // Register database context factory
        services.AddSingleton<HyperCubeDbContextFactory>();

        // Process connection string to replace environment variables
        var connectionString = config.ConnectionString.ReplaceEnvVariable();

        // Set up database context based on provider
        switch (config.DatabaseProvider)
        {
            case DatabaseProviderType.Sqlite:
                services.AddDbContext<HyperCubeDbContext>(options =>
                {
                    options.UseSqlite(
                        connectionString,
                        sqlite =>
                        {
                            sqlite.MigrationsAssembly(typeof(HyperCubeDbContext).Assembly.FullName);
                            sqlite.CommandTimeout(config.CommandTimeoutSeconds);
                        });

                    ConfigureDbContextOptions(options, config);
                });
                break;

            case DatabaseProviderType.SqlServer:
                services.AddDbContext<HyperCubeDbContext>(options =>
                {
                    options.UseSqlServer(
                        connectionString,
                        sqlServer =>
                        {
                            sqlServer.MigrationsAssembly(typeof(HyperCubeDbContext).Assembly.FullName);
                            sqlServer.CommandTimeout(config.CommandTimeoutSeconds);
                            sqlServer.MaxBatchSize(config.MaxBatchSize);
                            sqlServer.EnableRetryOnFailure(config.MaxRetryCount);
                        });

                    ConfigureDbContextOptions(options, config);
                });
                break;

            case DatabaseProviderType.PostgreSql:
                services.AddDbContext<HyperCubeDbContext>(options =>
                {
                    options.UseNpgsql(
                        connectionString,
                        npgsql =>
                        {
                            npgsql.MigrationsAssembly(typeof(HyperCubeDbContext).Assembly.FullName);
                            npgsql.CommandTimeout(config.CommandTimeoutSeconds);
                            npgsql.MaxBatchSize(config.MaxBatchSize);
                            npgsql.EnableRetryOnFailure(config.MaxRetryCount);
                        });

                    ConfigureDbContextOptions(options, config);
                });
                break;

            case DatabaseProviderType.MySql:
                services.AddDbContext<HyperCubeDbContext>(options =>
                {
                    options.UseMySql(
                        connectionString,
                        ServerVersion.AutoDetect(connectionString),
                        mysql =>
                        {
                            mysql.MigrationsAssembly(typeof(HyperCubeDbContext).Assembly.FullName);
                            mysql.CommandTimeout(config.CommandTimeoutSeconds);
                            mysql.EnableRetryOnFailure(config.MaxRetryCount);
                        });

                    ConfigureDbContextOptions(options, config);
                });
                break;

            case DatabaseProviderType.InMemory:
                services.AddDbContext<HyperCubeDbContext>(options =>
                {
                    options.UseInMemoryDatabase("HyperCubeInMemory");

                    ConfigureDbContextOptions(options, config);
                });
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(config.DatabaseProvider),
                    config.DatabaseProvider, "Unsupported database provider");
        }

        // Register generic repository
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

        return services;
    }

    /// <summary>
    /// Registers a repository for a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TKey">The entity key type.</typeparam>
    /// <param name="services">The service collection to add the repository to.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddRepository<TEntity, TKey>(this IServiceCollection services)
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        services.AddScoped<IRepository<TEntity, TKey>, Repository<TEntity, TKey>>();
        return services;
    }

    /// <summary>
    /// Migrates the database if auto-migration is enabled in the configuration.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="force">Whether to force migration regardless of configuration.</param>
    /// <returns>The same service provider for chaining.</returns>
    public static IServiceProvider MigrateDatabase(this IServiceProvider serviceProvider, bool force = false)
    {
        var config = serviceProvider.GetRequiredService<DatabaseConfig>();

        if (config.AutoMigrate || force)
        {
            using var scope = serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<HyperCubeDbContext>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<HyperCubeDbContext>();

            logger.LogInformation("Running database migrations...");

            try
            {
                if (config.DatabaseProvider != DatabaseProviderType.InMemory)
                {
                    dbContext.Database.Migrate();
                }

                logger.LogInformation("Database migrations completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while running database migrations");
                throw;
            }
        }

        return serviceProvider;
    }

    /// <summary>
    /// Configures common DbContext options.
    /// </summary>
    private static void ConfigureDbContextOptions(DbContextOptionsBuilder options, DatabaseConfig config)
    {
        if (config.EnableSensitiveDataLogging)
        {
            options.EnableSensitiveDataLogging();
        }

        if (config.EnableDetailedErrors)
        {
            options.EnableDetailedErrors();
        }

        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
}
