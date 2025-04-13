using HyperCube.Postman.Base.Events;

namespace HyperCube.Server.Core.Data.Events.Server;

/// <summary>
///  Event that is triggered when the server is stopping.
/// </summary>
public record ServerStoppingEvent() : BasePostmanRecordEvent;
