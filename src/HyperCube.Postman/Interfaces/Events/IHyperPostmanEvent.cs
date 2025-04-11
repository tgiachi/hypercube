namespace HyperCube.Postman.Interfaces.Events;

/// <summary>
/// Base interface for all events that can be dispatched through the HyperCube Postman system.
/// </summary>
public interface IHyperPostmanEvent
{

    string Id { get; }
}
