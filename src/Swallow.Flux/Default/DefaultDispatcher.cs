namespace Swallow.Flux.Default;

/// <summary>
/// A default dispatcher that will dispatch a given <see cref="ICommand"/> to all registered store sequentially
/// </summary>
/// <param name="stores">The stores to dispatch all commands to</param>
/// <remarks>
/// Any exception thrown by an <see cref="IStore"/> is kept; each command is <em>always</em> dispatched to every <see cref="IStore"/>.
/// In the end, a <see cref="AggregateException"/> containing all stores' exceptions is thrown if needed.
/// </remarks>
public sealed class DefaultDispatcher(IEnumerable<IStore> stores) : IDispatcher
{
    /// <inheritdoc />
    public async Task Dispatch(ICommand command, CancellationToken cancellationToken = default)
    {
        var exceptions = new List<Exception>();
        foreach (var store in stores)
        {
            try
            {
                await store.Handle(command, cancellationToken);
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
        }

        if (exceptions.Count is not 0)
        {
            throw new AggregateException(exceptions);
        }
    }
}
