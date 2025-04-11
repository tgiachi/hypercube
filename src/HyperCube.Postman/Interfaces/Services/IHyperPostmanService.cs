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


    /// <summary>
    ///  Registers a listener for a specific event type.
    /// </summary>
    /// <param name="listener"></param>
    /// <typeparam name="TEvent"></typeparam>
    void Subscribe<TEvent>(ILetterListener<TEvent> listener)
        where TEvent : class, IHyperPostmanEvent;


    /// <summary>
    ///  Registers a listener for a specific event type.
    /// </summary>
    /// <param name="handler"></param>
    /// <typeparam name="TEvent"></typeparam>
    void Subscribe<TEvent>(Func<TEvent, Task> handler)
        where TEvent : class, IHyperPostmanEvent;

    /// <summary>
    ///  Unregisters a listener for a specific event type.
    /// </summary>
    /// <param name="listener"></param>
    /// <typeparam name="TEvent"></typeparam>
    void Unsubscribe<TEvent>(ILetterListener<TEvent> listener)
        where TEvent : class, IHyperPostmanEvent;

    /// <summary>
    ///  Dispatches an event to all registered listeners asynchronously.
    /// </summary>
    /// <param name="eventData"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
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
