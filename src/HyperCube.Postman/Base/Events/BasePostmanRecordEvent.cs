using HyperCube.Postman.Interfaces.Events;
using HyperCube.Postman.Interfaces.Services;

namespace HyperCube.Postman.Base.Events;

public record BasePostmanRecordEvent() : IHyperPostmanEvent
{
    public string Id { get; } = Guid.NewGuid().ToString();
}
