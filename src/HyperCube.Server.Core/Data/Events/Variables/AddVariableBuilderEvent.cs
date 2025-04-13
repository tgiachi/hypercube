using HyperCube.Postman.Interfaces.Events;

namespace HyperCube.Server.Core.Data.Events.Variables;

public record AddVariableBuilderEvent(string VariableName, Func<object> Builder) : IHyperPostmanEvent
{
    public string Id { get; } = Guid.NewGuid().ToString();
}
