using HyperCube.Server.Core.Data.Metrics.Diagnostic;
using HyperCube.Server.Core.Interfaces.Services.Base;

namespace HyperCube.Server.Core.Interfaces.Services;

public interface IDiagnosticService : IHyperLoadableService, IDisposable
{
    // Get current metrics
    Task<DiagnosticMetrics> GetCurrentMetricsAsync();

    // Observable for continuous monitoring
    IObservable<DiagnosticMetrics> Metrics { get; }

    // Get the PID file path
    string PidFilePath { get; }

    // Force collect diagnostics now
    Task CollectMetricsAsync();
}
