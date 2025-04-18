using HyperCube.Postman.Config;
using HyperCube.Postman.Extensions;
using HyperCube.Server.Core.Data.Directories.Base;
using HyperCube.Server.Core.Data.Internal;
using HyperCube.Server.Core.Data.Options.Base;
using HyperCube.Server.Core.Extensions;
using HyperCube.Server.Core.Interfaces.Modules;
using HyperCube.Server.Core.Services;
using HyperCube.Server.Core.Services.Manager;
using HyperCube.Server.Core.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HyperCube.Server.Core.Builder;

/// <summary>
/// A builder for creating and configuring HyperCube host applications.
/// </summary>
/// <typeparam name="TOptions">The type of server options, must inherit from BaseServerOptions.</typeparam>
/// <typeparam name="TDirEnum">The enum type that defines the directory structure.</typeparam>
public class HyperCubeHostBuilder<TOptions, TDirEnum>
    where TOptions : BaseServerOptions
    where TDirEnum : struct, Enum
{
    private IHost _host;

    private bool _isBuilt;
    private readonly string _applicationName;
    private readonly string _environmentName;
    private readonly TOptions _options;
    private HyperPostmanConfig _hyperPostmanConfig = new();

    /// <summary>
    /// Creates a new instance of the HyperCubeHostBuilder.
    /// </summary>
    /// <param name="applicationName">The name of the application.</param>
    /// <param name="environmentName">The environment name (e.g., Development, Production). Defaults to "Development".</param>
    /// <returns>A new HyperCubeHostBuilder instance.</returns>
    public static HyperCubeHostBuilder<TOptions, TDirEnum> Create(
        string applicationName, string environmentName = "Development"
    )
    {
        return new HyperCubeHostBuilder<TOptions, TDirEnum>(applicationName, environmentName);
    }

    /// <summary>
    /// Gets the underlying HostApplicationBuilder for advanced configuration.
    /// </summary>
    public HostApplicationBuilder HostBuilder { get; }

    /// <summary>
    /// Gets the server options.
    /// </summary>
    public TOptions Options => _options;

    /// <summary>
    /// Gets the directory configuration.
    /// </summary>
    public BaseDirectoriesConfig<TDirEnum> DirectoriesConfig { get; private set; }

    /// <summary>
    /// Gets the built IHost instance. Only available after Build() has been called.
    /// </summary>
    public IHost AppHost => _host ?? throw new InvalidOperationException("Host has not been built. Call Build() first.");

    /// <summary>
    /// Gets the IServiceProvider from the built host. Only available after Build() has been called.
    /// </summary>
    public IServiceProvider Services => AppHost.Services;

    private HyperCubeHostBuilder(string applicationName, string environmentName = "Development")
    {
        _applicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName));
        _environmentName = environmentName ?? throw new ArgumentNullException(nameof(environmentName));

        // Create the host application builder with the specified settings
        HostBuilder = Host.CreateApplicationBuilder(
            new HostApplicationBuilderSettings
            {
                Args = Environment.GetCommandLineArgs(),
                ApplicationName = applicationName,
                EnvironmentName = environmentName
            }
        );

        HostBuilder.Services.AddSingleton(new AppDefinitionObject(_applicationName, _environmentName));

        // Parse command line arguments into options
        _options = Environment
            .GetCommandLineArgs()
            .ParseOptionCommandLine<TOptions>();

        // Register options with DI
        HostBuilder.Services.AddSingleton(_options);


        // Initialize directories
        InitializeDirectories();
    }

    public HyperCubeHostBuilder<TOptions, TDirEnum> AddHyperPostmanConfig(HyperPostmanConfig hyperPostmanConfig)
    {
        _hyperPostmanConfig = hyperPostmanConfig;
        return this;
    }

    /// <summary>
    /// Initializes the directory configuration and registers it with DI.
    /// </summary>
    private void InitializeDirectories()
    {
        DirectoriesConfig = _options.CreateDirectoryConfig<TDirEnum, TOptions>();
        HostBuilder.Services.AddSingleton(DirectoriesConfig);
    }

    /// <summary>
    /// Adds a module to the host.
    /// </summary>
    /// <typeparam name="TModule">The type of module to add, must implement IHyperCubeContainerModule.</typeparam>
    /// <returns>The current builder instance for chaining.</returns>
    public HyperCubeHostBuilder<TOptions, TDirEnum> AddModule<TModule>()
        where TModule : class, IHyperCubeContainerModule
    {
        HostBuilder.Services.AddModule<TModule>();
        return this;
    }

    /// <summary>
    /// Adds a module to the host by type.
    /// </summary>
    /// <param name="moduleType">The type of module to add. Must implement IHyperCubeContainerModule.</param>
    /// <returns>The current builder instance for chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the module type does not implement IHyperCubeContainerModule.</exception>
    public HyperCubeHostBuilder<TOptions, TDirEnum> AddModule(Type moduleType)
    {
        if (!typeof(IHyperCubeContainerModule).IsAssignableFrom(moduleType))
        {
            throw new ArgumentException(
                $"Module type must implement {nameof(IHyperCubeContainerModule)}",
                nameof(moduleType)
            );
        }

        HostBuilder.Services.AddModule(moduleType);
        return this;
    }

    /// <summary>
    /// Adds a service to the host.
    /// </summary>
    /// <typeparam name="TService">The service interface type.</typeparam>
    /// <typeparam name="TImplementation">The service implementation type.</typeparam>
    /// <param name="lifetimeType">The lifetime of the service (Singleton, Scoped, or Transient).</param>
    /// <returns>The current builder instance for chaining.</returns>
    public HyperCubeHostBuilder<TOptions, TDirEnum> AddService<TService, TImplementation>(
        ServiceLifetimeType lifetimeType = ServiceLifetimeType.Singleton
    )
        where TService : class
        where TImplementation : class, TService
    {
        HostBuilder.Services.AddService<TService, TImplementation>(lifetimeType);
        return this;
    }

    /// <summary>
    /// Configures the service collection using a delegate.
    /// </summary>
    /// <param name="configureServices">A delegate to configure the service collection.</param>
    /// <returns>The current builder instance for chaining.</returns>
    public HyperCubeHostBuilder<TOptions, TDirEnum> ConfigureServices(Action<IServiceCollection> configureServices)
    {
        configureServices.Invoke(HostBuilder.Services);
        return this;
    }

    /// <summary>
    /// Builds the host with the configured options and services.
    /// </summary>
    /// <returns>The current builder instance for chaining.</returns>
    public HyperCubeHostBuilder<TOptions, TDirEnum> Build()
    {
        if (_isBuilt)
        {
            throw new InvalidOperationException("Host has already been built. Call Build() only once.");
        }

        _isBuilt = true;

        HostBuilder.Services.AddHostedService<HyperCubeServiceManager>();
        HostBuilder.Services.AddPostman(_hyperPostmanConfig);

        // Configure logging
        HostBuilder.BuildLogger(DirectoriesConfig, _options);

        // Display header if enabled
        _options.ShowHeader();

        // Build the host
        _host = HostBuilder.Build();

        return this;
    }

    /// <summary>
    /// Runs the host asynchronously until shutdown is triggered.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the host execution.</returns>
    /// <exception cref="InvalidOperationException">Thrown if Build() has not been called before RunAsync().</exception>
    public Task RunAsync(CancellationToken cancellationToken = default)
    {
        if (_host == null)
        {
            throw new InvalidOperationException("Host has not been built. Call Build() before RunAsync().");
        }

        return _host.RunAsync(cancellationToken);
    }

    /// <summary>
    /// Starts the host and returns a task that represents the host lifetime.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the host lifetime.</returns>
    /// <exception cref="InvalidOperationException">Thrown if Build() has not been called before StartAsync().</exception>
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_host == null)
        {
            throw new InvalidOperationException("Host has not been built. Call Build() before StartAsync().");
        }

        return _host.StartAsync(cancellationToken);
    }

    /// <summary>
    /// Stops the host gracefully.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the stop operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if Build() has not been called before StopAsync().</exception>
    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_host == null)
        {
            throw new InvalidOperationException("Host has not been built. Call Build() before StopAsync().");
        }

        return _host.StopAsync(cancellationToken);
    }
}
