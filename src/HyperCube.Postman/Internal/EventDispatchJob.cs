using HyperCube.Postman.Interfaces.Events;
using HyperCube.Postman.Interfaces.Services;


namespace HyperCube.Postman.Internal;

internal abstract class EventDispatchJob
{
    public abstract Task ExecuteAsync();
}

/// <summary>
/// Generic implementation of event dispatch job
/// </summary>
internal class EventDispatchJob<TEvent> : EventDispatchJob
    where TEvent : class, IHyperPostmanEvent
{
    private readonly ILetterListener<TEvent> _listener;
    private readonly TEvent _event;


    public EventDispatchJob(ILetterListener<TEvent> listener, TEvent @event)
    {
        _listener = listener;
        _event = @event;
    }

    public override async Task ExecuteAsync()
    {
        await _listener.HandleAsync(_event);
    }
}
