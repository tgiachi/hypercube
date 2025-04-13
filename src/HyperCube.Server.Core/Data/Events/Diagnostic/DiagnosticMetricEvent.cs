using HyperCube.Postman.Base.Events;
using HyperCube.Server.Core.Data.Metrics.Diagnostic;

namespace HyperCube.Server.Core.Data.Events.Diagnostic;

public record DiagnosticMetricEvent(DiagnosticMetrics Metrics) : BasePostmanRecordEvent;
