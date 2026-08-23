namespace Swallow.Flux;

/// <summary>
/// A utility class to help you implement an <see cref="IStore"/>
/// </summary>
/// <param name="emitter">The emitter to use to emit <see cref="INotification"/>s</param>
/// <remarks>
/// Use <see cref="Register{TCommand}(System.Action{TCommand})"/> (or any other overload) in the constructor to define which <see cref="ICommand"/>s
/// to handle and how to handle them. A single command can have multiple handlers registered; they will all be invoked sequentially.
/// </remarks>
public abstract partial class AbstractStore(IEmitter emitter) : IStore
{
    private readonly List<(Type, Func<object, CancellationToken, Task>)> commandHandlers = [];

    /// <inheritdoc />
    public async Task Handle(ICommand command, CancellationToken cancellationToken = default)
    {
        foreach (var (type, handler) in commandHandlers)
        {
            if (type.IsInstanceOfType(command))
            {
                await handler.Invoke(command, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Emit the given <see cref="INotification"/> via the current <see cref="IEmitter"/>
    /// </summary>
    /// <param name="notification">The notification to emit</param>
    /// <seealso cref="IEmitter.Emit"/>
    protected void Emit(INotification notification)
    {
        emitter.Emit(notification);
    }

    /// <summary>
    /// Emit the given <see cref="INotification"/> via the current <see cref="IEmitter"/>
    /// </summary>
    /// <typeparam name="T">The type of notification to default-construct and emit</typeparam>
    /// <seealso cref="IEmitter.Emit{TNotification}"/>
    protected void Emit<T>() where T : INotification, new()
    {
        emitter.Emit<T>();
    }

    /// <summary>
    /// Register <paramref name="handler"/> to be executed when a command of type <typeparamref name="TCommand"/> is being handled
    /// </summary>
    /// <param name="handler">The handler to register</param>
    /// <typeparam name="TCommand">Type of command to handle</typeparam>
    protected void Register<TCommand>(Func<TCommand, CancellationToken, Task> handler) where TCommand : ICommand
    {
        commandHandlers.Add((typeof(TCommand), (command, cancellation) => handler((TCommand)command, cancellation)));
    }
}
