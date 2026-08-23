namespace Swallow.Flux;

/// <summary>
/// A simple binding, allowing you to react to different <see cref="INotification"/>s
/// </summary>
/// <seealso cref="ITargetedBinding{T}"/>
public interface IBinding
{
    /// <summary>
    /// Subscribe to notifications of type <typeparamref name="TNotification"/>
    /// </summary>
    /// <typeparam name="TNotification">Type of notification to subscribe to</typeparam>
    /// <param name="reaction">The action to execute on every received notifaction</param>
    /// <param name="immediatelyInvoke">Whether to immediately invoke <paramref name="reaction"/> after binding</param>
    /// <returns>The same <see cref="IBinding"/> for further configuration</returns>
    IBinding To<TNotification>(Action reaction, bool immediatelyInvoke = false) where TNotification : INotification;

    /// <summary>
    /// Subscribe to notifications of type <typeparamref name="TNotification"/>
    /// </summary>
    /// <typeparam name="TNotification">Type of notification to subscribe to</typeparam>
    /// <param name="reaction">The action to execute on every received notifaction</param>
    /// <returns>The same <see cref="IBinding"/> for further configuration</returns>
    IBinding To<TNotification>(Action<TNotification> reaction) where TNotification : INotification;

    /// <summary>
    /// Subscribe to notifications of type <typeparamref name="TNotification"/>
    /// </summary>
    /// <typeparam name="TNotification">Type of notification to subscribe to</typeparam>
    /// <param name="reaction">The action to execute on every received notifaction</param>
    /// <param name="immediatelyInvoke">Whether to immediately invoke <paramref name="reaction"/> after binding</param>
    /// <returns>The same <see cref="IBinding"/> for further configuration</returns>
    /// <remarks>
    /// A default-constructed <typeparamref name="TNotification"/> will be passed to <paramref name="reaction"/>.
    /// </remarks>
    IBinding To<TNotification>(Action<TNotification> reaction, bool immediatelyInvoke) where TNotification : INotification, new();
}
