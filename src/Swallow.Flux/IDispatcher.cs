namespace Swallow.Flux;

/// <summary>
/// A dispatcher to send an <see cref="ICommand"/> to all registered <see cref="IStore"/>s
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// Dispatch the given command to all stores
    /// </summary>
    /// <param name="command">The command to dispatch</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the operation</param>
    /// <seealso cref="Dispatch{T}"/>
    Task Dispatch(ICommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatch the given command to all stores
    /// </summary>
    /// <typeparam name="T">Type of command to default-construct and dispatch</typeparam>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the operation</param>
    /// <seealso cref="Dispatch"/>
    Task Dispatch<T>(CancellationToken cancellationToken = default) where T : ICommand, new() => Dispatch(new T(), cancellationToken);
}
