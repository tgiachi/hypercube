using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using HyperCube.Postman.Interfaces.Services;
using HyperCube.Server.Core.Data.Configs.Sections;
using HyperCube.Server.Core.Data.Events.Diagnostic;
using HyperCube.Server.Core.Data.Internal;
using HyperCube.Server.Core.Data.Metrics.Diagnostic;
using HyperCube.Server.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace HyperCube.Server.Core.Services;

public class DiagnosticService : IDiagnosticService
{
    public string PidFilePath { get; }


    private readonly ILogger<DiagnosticService> _logger;

    private readonly IHyperPostmanService _abyssSignalService;


    private readonly Subject<DiagnosticMetrics> _metricsSubject = new();

    private readonly IDisposable _timerSubscription;

    private readonly DiagnosticServiceConfig _diagnosticService;

    private long _uptimeStopwatch;
    private readonly Process _currentProcess;


    private int _lastGcGen0;
    private int _lastGcGen1;
    private int _lastGcGen2;

    public IObservable<DiagnosticMetrics> Metrics => _metricsSubject.AsObservable();


    public DiagnosticService(
        ILogger<DiagnosticService> logger, AppDefinitionObject appDefinitionObject,
        IHyperPostmanService abyssSignalService, DiagnosticServiceConfig diagnosticService
    )
    {
        _abyssSignalService = abyssSignalService;
        _diagnosticService = diagnosticService;
        _logger = logger;

        PidFilePath = Path.Combine(_diagnosticService.RootDirectory, $"{appDefinitionObject.ApplicationName}.pid");
        _currentProcess = Process.GetCurrentProcess();

        // Initialize GC collection counts
        _lastGcGen0 = GC.CollectionCount(0);
        _lastGcGen1 = GC.CollectionCount(1);
        _lastGcGen2 = GC.CollectionCount(2);


        _timerSubscription = Observable
            .Interval(TimeSpan.FromSeconds(60))
            .Subscribe(
                async void (_) =>
                {
                    try
                    {
                        await CollectMetricsAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error collecting metrics");
                    }
                }
            );
    }


    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _uptimeStopwatch = Stopwatch.GetTimestamp();
        WritePidFile();


        _logger.LogInformation("Diagnostic service started. PID: {Pid}", _currentProcess.Id);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        DeletePidFile();
        _metricsSubject.Dispose();
        _logger.LogInformation("Diagnostic service stopped");
    }

    public async Task<DiagnosticMetrics> GetCurrentMetricsAsync()
    {
        return await CollectMetricsInternalAsync();
    }

    public async Task CollectMetricsAsync()
    {
        var metrics = await CollectMetricsInternalAsync();
        _metricsSubject.OnNext(metrics);
        await _abyssSignalService.PublishAsync(new DiagnosticMetricEvent(metrics));
    }

    private async Task<DiagnosticMetrics> CollectMetricsInternalAsync()
    {
        var currentGen0 = GC.CollectionCount(0);
        var currentGen1 = GC.CollectionCount(1);
        var currentGen2 = GC.CollectionCount(2);

        var metrics = new DiagnosticMetrics(
            privateMemoryBytes: _currentProcess.WorkingSet64,
            pagedMemoryBytes: GC.GetTotalMemory(false),
            threadCount: _currentProcess.Threads.Count,
            processId: _currentProcess.Id,
            uptime: Stopwatch.GetElapsedTime(_uptimeStopwatch),
            cpuUsagePercent: 0,
            gcGen0Collections: currentGen0 - _lastGcGen0,
            gcGen1Collections: currentGen1 - _lastGcGen1,
            gcGen2Collections: currentGen2 - _lastGcGen2
        );

        // Update last GC collection counts
        _lastGcGen0 = currentGen0;
        _lastGcGen1 = currentGen1;
        _lastGcGen2 = currentGen2;

        return metrics;
    }

    private void WritePidFile()
    {
        try
        {
            File.WriteAllText(PidFilePath, _currentProcess.Id.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write PID file");
        }
    }

    private void DeletePidFile()
    {
        try
        {
            if (File.Exists(PidFilePath))
            {
                File.Delete(PidFilePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete PID file");
        }
    }

    public void Dispose()
    {
        _metricsSubject.Dispose();
        _timerSubscription.Dispose();
        _currentProcess.Dispose();
    }
}
