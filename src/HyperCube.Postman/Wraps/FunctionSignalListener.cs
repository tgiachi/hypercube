using HyperCube.Postman.Interfaces.Events;
using HyperCube.Postman.Interfaces.Services;

namespace HyperCube.Postman.Wraps;

/// <summary>
/// Adapter class that wraps a function to implement IAbyssSignalListener
/// </summary>
public class FunctionSignalListener<TEvent> : ILetterListener<TEvent>
    where TEvent : class, IHyperPostmanEvent
{
    private readonly Func<TEvent, Task> _handler;

    public FunctionSignalListener(Func<TEvent, Task> handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public Task HandleAsync(TEvent signalEvent, CancellationToken cancellationToken = default)
    {
        return _handler(signalEvent);
    }

    /// <summary>
    /// Checks if this wrapper contains the same handler function
    /// </summary>
    public bool HasSameHandler(Func<TEvent, Task> handler)
    {
        return _handler.Equals(handler);
    }
}
