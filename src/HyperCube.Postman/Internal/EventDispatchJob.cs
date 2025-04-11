using HyperCube.Postman.Interfaces.Events;
using HyperCube.Postman.Interfaces.Services;
using Serilog;
using ILogger = Serilog.ILogger;

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
    private static readonly ILogger Logger = Log.ForContext<EventDispatchJob<TEvent>>();

    public EventDispatchJob(ILetterListener<TEvent> listener, TEvent @event)
    {
        _listener = listener;
        _event = @event;
    }

    public override async Task ExecuteAsync()
    {
        try
        {
            await _listener.HandleAsync(_event);
        }
        catch (Exception ex)
        {
            Logger.Error(
                ex,
                "Error dispatching event {EventType} to listener {ListenerType}",
                typeof(TEvent).Name,
                _listener.GetType().Name
            );
        }
    }
}
