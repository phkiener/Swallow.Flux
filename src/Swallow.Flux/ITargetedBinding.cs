namespace Swallow.Flux;

/// <summary>
/// A binding targeting a specific object, allowing it to react to different <see cref="INotification"/>s
/// </summary>
/// <typeparam name="T">Type of object that will react to the notifications</typeparam>
/// <seealso cref="IBinding"/>
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
