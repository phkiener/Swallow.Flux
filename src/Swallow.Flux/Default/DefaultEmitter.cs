namespace Swallow.Flux.Default;

/// <summary>
/// A default emitter that directly invokes <see cref="OnEmit"/>
/// </summary>
public sealed class DefaultEmitter : IEmitter
{
    /// <inheritdoc />
    public void Emit(INotification notification)
    {
        OnEmit?.Invoke(this, notification);
    }

    /// <inheritdoc />
    public event EventHandler<INotification>? OnEmit;
}
