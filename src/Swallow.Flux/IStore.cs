namespace Swallow.Flux;

/// <summary>
/// Marker interface for a command that may be dispatched by an <see cref="IDispatcher"/> and handled by an <see cref="IStore"/>
/// </summary>
public interface ICommand;

/// <summary>
/// A store that keeps its own state and reacts to different <see cref="ICommand"/>s
/// </summary>
public interface IStore
{
    /// <summary>
    /// Handle the given command, updating the internal state - or ignore it if the command is not relevant
    /// </summary>
    /// <param name="command">The command to handle</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the operation</param>
    Task Handle(ICommand command, CancellationToken cancellationToken = default);
}
