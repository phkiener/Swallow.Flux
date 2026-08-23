using Swallow.Flux.Default;

namespace Swallow.Flux;

public partial class AbstractStoreTest
{
    [Test]
    public async Task HandlersOfAllTypes_InvokedWithCorrectParameters()
    {
        var store = new RegistrationsStore(new DefaultEmitter());
        var dispatcher = new DefaultDispatcher([store]);

        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        await dispatcher.Dispatch(new DummyCommand { IsSet = true }, cancellationTokenSource.Token);

        await Assert.That(store.HandlersCalled).IsEqualTo(8);
    }

    private sealed class RegistrationsStore : AbstractStore
    {
        public int HandlersCalled { get; private set; }

        public RegistrationsStore(IEmitter emitter) : base(emitter)
        {
            Register<DummyCommand>(InvokeSyncNoParameters);
            Register<DummyCommand>(InvokeSyncCommand);
            Register<DummyCommand>(InvokeSyncToken);
            Register<DummyCommand>(InvokeSyncBoth);
            Register<DummyCommand>(InvokeAsyncNoParameters);
            Register<DummyCommand>(InvokeAsyncCommand);
            Register<DummyCommand>(InvokeAsyncToken);
            Register<DummyCommand>(InvokeAsyncBoth);
        }

        private void InvokeSyncNoParameters()
        {
            HandlersCalled += 1;
        }

        private void InvokeSyncCommand(DummyCommand obj)
        {
            AssertCommand(obj);

            HandlersCalled += 1;
        }

        private void InvokeSyncToken(CancellationToken obj)
        {
            AssertToken(obj);

            HandlersCalled += 1;
        }

        private void InvokeSyncBoth(DummyCommand arg1, CancellationToken arg2)
        {
            AssertCommand(arg1);
            AssertToken(arg2);

            HandlersCalled += 1;
        }

        private Task InvokeAsyncNoParameters()
        {
            HandlersCalled += 1;

            return Task.CompletedTask;
        }

        private Task InvokeAsyncCommand(DummyCommand arg)
        {
            AssertCommand(arg);

            HandlersCalled += 1;

            return Task.CompletedTask;
        }

        private Task InvokeAsyncToken(CancellationToken arg)
        {
            AssertToken(arg);

            HandlersCalled += 1;

            return Task.CompletedTask;
        }

        private Task InvokeAsyncBoth(DummyCommand arg1, CancellationToken arg2)
        {
            AssertCommand(arg1);
            AssertToken(arg2);

            HandlersCalled += 1;

            return Task.CompletedTask;
        }

        private static void AssertCommand(DummyCommand command)
        {
            if (!command.IsSet)
            {
                Assert.Fail("Incorrect command received");
            }
        }

        private static void AssertToken(CancellationToken token)
        {
            if (!token.IsCancellationRequested)
            {
                Assert.Fail("Incorrect cancellation token received");
            }
        }
    }

    private sealed class DummyCommand : ICommand
    {
        public bool IsSet { get; init; } = false;
    }
}
