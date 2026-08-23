namespace Swallow.Flux;

/// <summary>
/// Marker interface for a notification that may be emitted by an <see cref="IEmitter"/> and subscribed to
/// </summary>
public interface INotification;

/// <summary>
/// An emitter to publish <see cref="INotification"/>s to interested subscribers
/// </summary>
public interface IEmitter
{
    /// <summary>
    /// Notify all subscribers about the given notification
    /// </summary>
    /// <param name="notification">The notification to push to subscribers</param>
    /// <seealso cref="Emit{T}"/>
    void Emit(INotification notification);

    /// <summary>
    /// Notify all subscribers about the given notification
    /// </summary>
    /// <typeparam name="T">Type of notification to default-construct and emit</typeparam>
    /// <seealso cref="Emit"/>
    void Emit<T>() where T : INotification, new() => Emit(new T());

    /// <summary>
    /// Event that is invoked on every <see cref="Emit"/> or <see cref="Emit{T}"/>
    /// </summary>
    event EventHandler<INotification>? OnEmit;
}
