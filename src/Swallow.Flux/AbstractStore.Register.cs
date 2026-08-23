namespace Swallow.Flux;

// 1. No parameters
// 2. Command as parameter
// 3. CancellationToken as parameter
// 4. Command and CancellationToken as parameters
//
// Async version of 4. is considered the canonical version and defined in the "main" file

public partial class AbstractStore
{
    #region sync

    /// <inheritdoc cref="Register{TCommand}(System.Func{TCommand, CancellationToken, Task})"/>
    protected void Register<TCommand>(Action handler) where TCommand : ICommand
    {
        commandHandlers.Add((typeof(TCommand), RunHandler(handler)));
        return;

        static Func<object, CancellationToken, Task> RunHandler(Action handler)
        {
            return (_, _) =>
            {
                handler.Invoke();
                return Task.CompletedTask;
            };
        }
    }

    /// <inheritdoc cref="Register{TCommand}(System.Func{TCommand, CancellationToken, Task})"/>
    protected void Register<TCommand>(Action<TCommand> handler) where TCommand : ICommand
    {
        commandHandlers.Add((typeof(TCommand), RunHandler(handler)));
        return;

        static Func<object, CancellationToken, Task> RunHandler(Action<TCommand> handler)
        {
            return (command, _) =>
            {
                handler.Invoke((TCommand)command);
                return Task.CompletedTask;
            };
        }
    }

    /// <inheritdoc cref="Register{TCommand}(System.Func{TCommand, CancellationToken, Task})"/>
    protected void Register<TCommand>(Action<CancellationToken> handler) where TCommand : ICommand
    {
        commandHandlers.Add((typeof(TCommand), RunHandler(handler)));
        return;

        static Func<object, CancellationToken, Task> RunHandler(Action<CancellationToken> handler)
        {
            return (_, token) =>
            {
                handler.Invoke(token);
                return Task.CompletedTask;
            };
        }
    }

    /// <inheritdoc cref="Register{TCommand}(System.Func{TCommand, CancellationToken, Task})"/>
    protected void Register<TCommand>(Action<TCommand, CancellationToken> handler) where TCommand : ICommand
    {
        commandHandlers.Add((typeof(TCommand), RunHandler(handler)));
        return;

        static Func<object, CancellationToken, Task> RunHandler(Action<TCommand, CancellationToken> handler)
        {
            return (command, token) =>
            {
                handler.Invoke((TCommand)command, token);
                return Task.CompletedTask;
            };
        }
    }

    #endregion

    #region async

    /// <inheritdoc cref="Register{TCommand}(System.Func{TCommand, CancellationToken, Task})"/>
    protected void Register<TCommand>(Func<Task> handler) where TCommand : ICommand
    {
        commandHandlers.Add((typeof(TCommand), (_, _) => handler()));
    }

    /// <inheritdoc cref="Register{TCommand}(System.Func{TCommand, CancellationToken, Task})"/>
    protected void Register<TCommand>(Func<TCommand, Task> handler) where TCommand : ICommand
    {
        commandHandlers.Add((typeof(TCommand), (command, _) => handler((TCommand)command)));
    }

    /// <inheritdoc cref="Register{TCommand}(System.Func{TCommand, CancellationToken, Task})"/>
    protected void Register<TCommand>(Func<CancellationToken, Task> handler) where TCommand : ICommand
    {
        commandHandlers.Add((typeof(TCommand), (_, token) => handler(token)));
    }

    #endregion

}
