namespace HyperCube.Postman.Config;

/// <summary>
/// Configuration for the HyperPostman service.
/// </summary>
public class HyperPostmanConfig
{
    /// <summary>
    /// Gets or sets the maximum number of concurrent tasks used for event dispatching.
    /// </summary>
    /// <remarks>
    /// This limits the parallelism of event handling. Set to 0 or a negative number
    /// to use the default level of parallelism (usually equal to Environment.ProcessorCount).
    /// </remarks>
    public int MaxConcurrentTasks { get; set; } = 0;

    /// <summary>
    /// Gets or sets whether to continue dispatching other events when one fails.
    /// </summary>
    public bool ContinueOnError { get; set; } = true;

    /// <summary>
    /// Gets or sets the timeout in milliseconds for event handling.
    /// </summary>
    /// <remarks>
    /// Set to 0 for no timeout.
    /// </remarks>
    public int TimeoutMilliseconds { get; set; } = 30000;

    /// <summary>
    /// Gets or sets whether to buffer events if dispatch is busy.
    /// </summary>
    public bool BufferEvents { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum size of the event buffer when BufferEvents is true.
    /// </summary>
    public int MaxBufferSize { get; set; } = 1000;
}
