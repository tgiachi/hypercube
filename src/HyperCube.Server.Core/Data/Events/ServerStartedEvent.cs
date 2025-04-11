using HyperCube.Postman.Base.Events;

namespace HyperCube.Server.Core.Data.Events;

/// <summary>
///  Event that is triggered when the server starts.
/// </summary>
public record ServerStartedEvent() : BasePostmanRecordEvent(Guid.NewGuid().ToString());
