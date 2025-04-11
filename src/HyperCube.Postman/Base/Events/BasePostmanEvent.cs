using HyperCube.Postman.Interfaces.Events;

namespace HyperCube.Postman.Base.Events;

/// <summary>
/// Base implementation of IHyperPostmanEvent that can be extended by specific event types.
/// </summary>
public abstract class BasePostmanEvent : IHyperPostmanEvent
{
    public string Id { get; } = Guid.NewGuid().ToString().Replace("-", "");
}
