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

}
