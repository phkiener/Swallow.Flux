namespace Swallow.Flux.Default;

public sealed class DefaultDispatcherTest
{
    [Test]
    public async Task NoStores_DoesNothing()
    {
        var dispatcher = new DefaultDispatcher([]);

        await Assert.That(() => dispatcher.Dispatch(new DummyCommand())).ThrowsNothing();
    }

    [Test]
    public async Task MultipleStores_InvokesStoresInOrder()
    {
        var firstStore = new DummyStore();
        var secondStore = new DummyStore();

        var dispatcher = new DefaultDispatcher([firstStore, secondStore]);
        await dispatcher.Dispatch(new DummyCommand());

        await Assert.That(firstStore.LastCommand).IsNotDefault();
        await Assert.That(secondStore.LastCommand).IsNotDefault();
        await Assert.That(secondStore.LastCommand).IsGreaterThan(firstStore.LastCommand);
    }

    [Test]
    public async Task FirstStoreThrowsException_StillInvokesSecondStore()
    {
        var firstStore = new ThrowingStore();
        var secondStore = new DummyStore();

        var dispatcher = new DefaultDispatcher([firstStore, secondStore]);
        var exception = await Assert.ThrowsAsync<AggregateException>(() => dispatcher.Dispatch(new DummyCommand()));

        await Assert.That(secondStore.LastCommand).IsNotDefault();
        await Assert.That(exception!.InnerExceptions).Count().IsEqualTo(1);
        await Assert.That(exception!.InnerExceptions.Single()).IsAssignableTo<InvalidOperationException>().And.HasMessage("Doesn't work");
    }

    private sealed record DummyCommand : ICommand;

    private sealed class DummyStore : IStore
    {
        public DateTime LastCommand { get; private set; }

        public async Task Handle(ICommand command, CancellationToken cancellationToken = default)
        {
            LastCommand = DateTime.UtcNow;
            await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken); // to ensure the time comparison works
        }
    }

    private sealed class ThrowingStore : IStore
    {
        public Task Handle(ICommand command, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Doesn't work");
        }
    }
}
