using HyperCube.Postman.Interfaces.Events;

namespace HyperCube.Postman.Interfaces.Services;

/// <summary>
/// Interface for the HyperCube Postman service that dispatches events to registered listeners.
/// </summary>
public interface IHyperPostmanService
{
    /// <summary>
    ///  Observable that emits all events dispatched through the system.
    /// </summary>
    IObservable<object> AllEventsObservable { get; }

    void Subscribe<TEvent>(ILetterListener<TEvent> listener)
        where TEvent : class, IHyperPostmanEvent;

    void Subscribe<TEvent>(Func<TEvent, Task> handler)
        where TEvent : class, IHyperPostmanEvent;

    void Unsubscribe<TEvent>(ILetterListener<TEvent> listener)
        where TEvent : class, IHyperPostmanEvent;

    Task PublishAsync<TEvent>(TEvent eventData, CancellationToken cancellationToken = default)
        where TEvent : class, IHyperPostmanEvent;

    /// <summary>
    /// Gets the current number of registered listeners for all event types.
    /// </summary>
    /// <returns>The total number of registered listeners.</returns>
    int GetListenerCount();

    /// <summary>
    /// Gets the current number of registered listeners for a specific event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    /// <returns>The number of registered listeners for the specified event type.</returns>
    int GetListenerCount<TEvent>() where TEvent : IHyperPostmanEvent;


    /// <summary>
    ///  Waits for all dispatched events to be processed.
    /// </summary>
    /// <returns></returns>
    public Task WaitForCompletionAsync();
}
