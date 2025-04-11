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
    /// Registers a listener for a specific event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to listen for.</typeparam>
    /// <param name="listener">The listener to register.</param>
    void RegisterListener<TEvent>(ILetterListener<TEvent> listener) where TEvent : IHyperPostmanEvent;

    /// <summary>
    /// Registers a callback function for a specific event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to listen for.</typeparam>
    /// <param name="callback">The callback function to register.</param>
    /// <returns>A subscription that can be disposed to unregister the callback.</returns>
    IDisposable RegisterCallback<TEvent>(Func<TEvent, CancellationToken, Task> callback) where TEvent : IHyperPostmanEvent;

    /// <summary>
    /// Dispatches an event to all registered listeners and callbacks.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to dispatch.</typeparam>
    /// <param name="event">The event to dispatch.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IHyperPostmanEvent;

    /// <summary>
    /// Unregisters a listener for a specific event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    /// <param name="listener">The listener to unregister.</param>
    void UnregisterListener<TEvent>(ILetterListener<TEvent> listener) where TEvent : IHyperPostmanEvent;

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
}
