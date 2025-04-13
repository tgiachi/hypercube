namespace HyperCube.Server.Core.Interfaces.Services.Base;

/// <summary>
/// Marker interface for services that should be automatically loaded and registered at application startup.
/// </summary>
/// <remarks>
/// Services that implement this interface will be discovered through reflection and
/// automatically registered with the dependency injection container during application initialization.
/// No additional manual registration is required for these services.
///
/// Example usage:
/// <code>
/// public class MyBackgroundService : IHostedService, IHyperAutoLoadService
/// {
///     // Implementation
/// }
/// </code>
/// </remarks>
public interface IHyperAutoLoadService
{
    // This is a marker interface with no members
}
