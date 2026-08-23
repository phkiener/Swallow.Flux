using Swallow.Flux.Default;

namespace Swallow.Flux;

public sealed partial class AbstractStoreTest
{
    [Test]
    public async Task InvokesRegisteredHandlers()
    {
        var store = new ImplementedStore(new DefaultEmitter());
        var dispatcher = new DefaultDispatcher([store]);

        await dispatcher.Dispatch(new AHappened());
        await dispatcher.Dispatch(new BHappened());
        await dispatcher.Dispatch(new BHappened());

        await Assert.That(store.CountA).IsEqualTo(1);
        await Assert.That(store.CountB).IsEqualTo(2);
        await Assert.That(store.CountAll).IsEqualTo(3);
    }

    private sealed class ImplementedStore : AbstractStore
    {
        public int CountA { get; private set; }
        public int CountB { get; private set; }
        public int CountAll { get; private set; }

        public ImplementedStore(IEmitter emitter) : base(emitter)
        {
            Register<AHappened>(IncrementA);
            Register<AHappened>(IncrementTotal);

            Register<BHappened>(IncrementB);
            Register<BHappened>(IncrementTotal);
        }

        private void IncrementA()
        {
            CountA += 1;
        }

        private void IncrementB()
        {
            CountB += 1;
        }

        private void IncrementTotal()
        {
            CountAll += 1;
        }
    }

    private sealed class AHappened : ICommand;
    private sealed class BHappened : ICommand;
}
