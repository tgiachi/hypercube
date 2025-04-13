using HyperCube.Postman.Interfaces.Events;

namespace HyperCube.Server.Core.Data.Events.Scheduler;

public abstract record AddSchedulerJobEvent(string Name, TimeSpan TotalSpan, Func<Task> Action) : IHyperPostmanEvent
{
    public string Id { get; } = Guid.NewGuid().ToString();
}
