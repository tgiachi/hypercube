namespace HyperCube.Postman.Wraps;

/// <summary>
/// Disposable class for subscription cleanup.
/// </summary>
public class SubscriptionDisposable : IDisposable
{
    private readonly Action _unsubscribeAction;
    private bool _disposed;

    public SubscriptionDisposable(Action unsubscribeAction)
    {
        _unsubscribeAction = unsubscribeAction;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _unsubscribeAction();
            _disposed = true;
        }
    }
}
