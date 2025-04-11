using HyperCube.Postman.Interfaces.Events;

namespace HyperCube.Postman.Interfaces.Services;

/// <summary>
/// Interface for listeners that want to receive and handle specific types of events.
/// </summary>
/// <typeparam name="TEvent">The type of event to listen for, must implement IHyperPostmanEvent.</typeparam>
public interface ILetterListener<in TEvent> where TEvent : IHyperPostmanEvent
{
    /// <summary>
    /// Handles the received event.
    /// </summary>
    /// <param name="event">The event to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}
