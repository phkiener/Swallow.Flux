namespace Swallow.Flux;

/// <summary>
/// A binding targeting a specific object, allowing it to react to different <see cref="INotification"/>s
/// </summary>
/// <typeparam name="T">Type of object that will react to the notifications</typeparam>
public interface ITargetedBinding<out T> where T : class
{
    /// <summary>
    /// Subscribe to notifications of type <typeparamref name="TNotification"/>
    /// </summary>
    /// <typeparam name="TNotification">Type of notification to subscribe to</typeparam>
    /// <param name="reaction">The action to execute on every received notifaction</param>
    /// <param name="immediatelyInvoke">Whether to immediately invoke <paramref name="reaction"/> after binding</param>
    /// <returns>The same <see cref="ITargetedBinding{T}"/> for further configuration</returns>
    ITargetedBinding<T> To<TNotification>(Action<T> reaction, bool immediatelyInvoke = false) where TNotification : INotification;

    /// <summary>
    /// Subscribe to notifications of type <typeparamref name="TNotification"/>
    /// </summary>
    /// <typeparam name="TNotification">Type of notification to subscribe to</typeparam>
    /// <param name="reaction">The action to execute on every received notifaction</param>
    /// <returns>The same <see cref="ITargetedBinding{T}"/> for further configuration</returns>
    ITargetedBinding<T> To<TNotification>(Action<T, TNotification> reaction) where TNotification : INotification;

    /// <summary>
    /// Subscribe to notifications of type <typeparamref name="TNotification"/>
    /// </summary>
    /// <typeparam name="TNotification">Type of notification to subscribe to</typeparam>
    /// <param name="reaction">The action to execute on every received notifaction</param>
    /// <param name="immediatelyInvoke">Whether to immediately invoke <paramref name="reaction"/> after binding</param>
    /// <returns>The same <see cref="ITargetedBinding{T}"/> for further configuration</returns>
    /// <remarks>
    /// A default-constructed <typeparamref name="TNotification"/> will be passed to <paramref name="reaction"/>.
    /// </remarks>
    ITargetedBinding<T> To<TNotification>(Action<T, TNotification> reaction, bool immediatelyInvoke) where TNotification : INotification, new();
}

/// <summary>
/// A binder to simplify subscribing to <see cref="INotification"/>s emitted by an <see cref="IEmitter"/>
/// </summary>
/// <remarks>
/// An implementation should dispose all registered bindings and subscriptions to notification when <see cref="IDisposable.Dispose()"/>d.
/// </remarks>
public interface IBinder : IDisposable
{
    /// <summary>
    /// Bind to the given <paramref name="target"/>, allowing it to subscribe to notifications
    /// </summary>
    /// <typeparam name="T">Type of object that is subscribing to notifications</typeparam>
    /// <param name="target">The object on which the subscribed notification should invoke actions on</param>
    /// <returns>A <see cref="ITargetedBinding{T}"/> to configure the subscriptions</returns>
    /// <seealso cref="Bind{T}(T, Action{Action})"/>
    ITargetedBinding<T> Bind<T>(T target) where T : class;

    /// <summary>
    /// Bind to the given <paramref name="target"/>, allowing it to subscribe to notifications
    /// </summary>
    /// <typeparam name="T">Type of object that is subscribing to notifications</typeparam>
    /// <param name="target">The object on which the subscribed notification should invoke actions on</param>
    /// <param name="wrapper">An action that should be used to wrap all reactions to a notification</param>
    /// <returns>A <see cref="ITargetedBinding{T}"/> to configure the subscriptions</returns>
    /// <seealso cref="Bind{T}(T)"/>
    /// <remarks>
    /// A <paramref name="wrapper"/> can be used to enforce execution on a UI thread, e.g. by passing in <c>InvokeAsync</c> (in Blazor)
    /// or <c>target.InvokeOnMainThread</c> (in macOS AppKit).
    /// </remarks>
    ITargetedBinding<T> Bind<T>(T target, Action<Action> wrapper) where T : class;
}
