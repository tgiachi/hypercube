using HyperCube.Postman.Interfaces.Services;
using HyperCube.Server.Core.Data.Events;
using HyperCube.Server.Core.Data.Events.Server;
using HyperCube.Server.Core.Interfaces.Services.Base;
using HyperCube.Server.Core.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HyperCube.Server.Core.Services.Manager;

public class HyperCubeServiceManager : IHostedService
{
    private readonly ILogger _logger;

    private readonly List<ServiceDefinitionObject> _serviceDefinitions;

    private readonly IServiceProvider _serviceProvider;

    private readonly IHyperPostmanService _hyperPostmanService;

    public HyperCubeServiceManager(
        ILogger<HyperCubeServiceManager> logger, List<ServiceDefinitionObject> serviceDefinitions,
        IServiceProvider serviceProvider, IHyperPostmanService hyperPostmanService
    )
    {
        _logger = logger;
        _serviceDefinitions = serviceDefinitions;
        _serviceProvider = serviceProvider;
        _hyperPostmanService = hyperPostmanService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var serviceType in _serviceDefinitions)
        {
            if (serviceType.IsAutoStart)
            {
                var service = _serviceProvider.GetService(serviceType.ServiceType);
                if (service is IHyperLoadableService autoLoadService)
                {
                    _logger.LogInformation("Starting service: {ServiceType}", serviceType.ServiceType.Name);
                    await autoLoadService.StartAsync(cancellationToken);
                }
            }
        }

        await _hyperPostmanService.PublishAsync(new ServerStartedEvent(), cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var serviceType in _serviceDefinitions)
        {
            if (serviceType.IsAutoStart)
            {
                var service = _serviceProvider.GetService(serviceType.ServiceType);
                if (service is IHyperLoadableService autoLoadService)
                {
                    _logger.LogInformation("Stopping service: {ServiceType}", serviceType.ServiceType.Name);
                    await autoLoadService.StopAsync(cancellationToken);
                }
            }
        }
    }
}
