namespace Swallow.Flux;

/// <summary>
/// A binder to simplify subscribing to <see cref="INotification"/>s emitted by an <see cref="IEmitter"/>
/// </summary>
/// <remarks>
/// An implementation should dispose all registered bindings and subscriptions to notification when <see cref="IDisposable.Dispose()"/>d.
/// </remarks>
public interface IBinder : IDisposable
{
    /// <summary>
    /// Create a new binding, allowing you to subscribe to notifications
    /// </summary>
    /// <returns>A <see cref="IBinder"/> to configure the subscriptions</returns>
    /// <seealso cref="Bind{T}(T, Action{Action})"/>
    IBinding Bind();

    /// <summary>
    /// Create a new binding, allowing you to subscribe to notifications
    /// </summary>
    /// <param name="wrapper">An action that should be used to wrap all reactions to a notification</param>
    /// <returns>A <see cref="IBinder"/> to configure the subscriptions</returns>
    /// <remarks>
    /// A <paramref name="wrapper"/> can be used to enforce execution on a UI thread, e.g. by passing in <c>InvokeAsync</c> (in Blazor)
    /// or <c>target.InvokeOnMainThread</c> (in macOS AppKit).
    /// </remarks>
    IBinding Bind(Action<Action> wrapper);

    /// <summary>
    /// Bind to the given <paramref name="target"/>, allowing it to subscribe to notifications
    /// </summary>
    /// <typeparam name="T">Type of object that is subscribing to notifications</typeparam>
    /// <param name="target">The object on which the subscribed notification should invoke actions on</param>
    /// <returns>A <see cref="ITargetedBinding{T}"/> to configure the subscriptions</returns>
    ITargetedBinding<T> Bind<T>(T target) where T : class;

    /// <summary>
    /// Bind to the given <paramref name="target"/>, allowing it to subscribe to notifications
    /// </summary>
    /// <typeparam name="T">Type of object that is subscribing to notifications</typeparam>
    /// <param name="target">The object on which the subscribed notification should invoke actions on</param>
    /// <param name="wrapper">An action that should be used to wrap all reactions to a notification</param>
    /// <returns>A <see cref="ITargetedBinding{T}"/> to configure the subscriptions</returns>
    /// <remarks>
    /// A <paramref name="wrapper"/> can be used to enforce execution on a UI thread, e.g. by passing in <c>InvokeAsync</c> (in Blazor)
    /// or <c>target.InvokeOnMainThread</c> (in macOS AppKit).
    /// </remarks>
    ITargetedBinding<T> Bind<T>(T target, Action<Action> wrapper) where T : class;
}
