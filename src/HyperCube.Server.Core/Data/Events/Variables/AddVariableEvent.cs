using HyperCube.Postman.Interfaces.Events;

namespace HyperCube.Server.Core.Data.Events.Variables;

public record AddVariableEvent(string VariableName, object Value) : IHyperPostmanEvent
{
    public string Id { get; } = Guid.NewGuid().ToString();
}
