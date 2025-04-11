using HyperCube.Postman.Interfaces.Events;
using HyperCube.Postman.Interfaces.Services;

namespace HyperCube.Postman.Wraps;

/// <summary>
/// Wrapper class to convert a callback function to an ILetterListener.
/// </summary>
/// <typeparam name="TEvent">The type of event to handle.</typeparam>
public class CallbackWrapper<TEvent> : ILetterListener<TEvent> where TEvent : IHyperPostmanEvent
{
    private readonly Func<TEvent, CancellationToken, Task> _callback;

    public CallbackWrapper(Func<TEvent, CancellationToken, Task> callback)
    {
        _callback = callback;
    }

    public Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default)
    {
        return _callback(@event, cancellationToken);
    }
}
