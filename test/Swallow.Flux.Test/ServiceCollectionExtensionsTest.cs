using Microsoft.Extensions.DependencyInjection;

namespace Swallow.Flux;

public sealed class ServiceCollectionExtensionsTest
{
    [Test]
    public async Task RegistersAllServices()
    {
        await using var serviceProvider = new ServiceCollection().AddFlux().BuildServiceProvider();

        await Assert.That(serviceProvider.GetService<IDispatcher>()).IsNotNull();
        await Assert.That(serviceProvider.GetService<IEmitter>()).IsNotNull();
        await Assert.That(serviceProvider.GetService<IBinder>()).IsNotNull();
    }

    [Test]
    public async Task ExistingRegistrationsAreRespected()
    {
        await using var serviceProvider = new ServiceCollection()
            .AddSingleton<IDispatcher, DummyDispatcher>()
            .AddSingleton<IEmitter, DummyEmitter>()
            .AddSingleton<IBinder, DummyBinder>()
            .AddFlux()
            .BuildServiceProvider();

        await Assert.That(serviceProvider.GetService<IDispatcher>()).IsTypeOf<DummyDispatcher>();
        await Assert.That(serviceProvider.GetService<IEmitter>()).IsTypeOf<DummyEmitter>();
        await Assert.That(serviceProvider.GetService<IBinder>()).IsTypeOf<DummyBinder>();
    }

    [Test]
    public async Task ConcreteStoreCanBeRegistered()
    {
        await using var serviceProvider = new ServiceCollection()
            .AddFlux()
            .AddStore<DummyStore>()
            .BuildServiceProvider();

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        await dispatcher.Dispatch<DummyCommand>();

        var store = serviceProvider.GetRequiredService<DummyStore>();
        await Assert.That(store.CommandsReceived).IsEqualTo(1);
    }

    [Test]
    public async Task StoreWithInterfaceCanBeRegistered()
    {
        await using var serviceProvider = new ServiceCollection()
            .AddFlux()
            .AddStore<IDummyStore, DummyStore>()
            .BuildServiceProvider();

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        await dispatcher.Dispatch<DummyCommand>();

        var store = serviceProvider.GetRequiredService<IDummyStore>();
        await Assert.That(store.CommandsReceived).IsEqualTo(1);
    }

    private sealed class DummyDispatcher : IDispatcher
    {
        public Task Dispatch(ICommand command, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }

    private sealed class DummyEmitter : IEmitter
    {
        public void Emit(INotification notification) => throw new NotImplementedException();

#pragma warning disable CS0067 // It's unused because it's just a dummy
        public event EventHandler<INotification>? OnEmit;
    }

    private sealed class DummyBinder : IBinder
    {
        public IBinding Bind() => throw new NotImplementedException();

        public IBinding Bind(Action<Action> wrapper) => throw new NotImplementedException();

        public ITargetedBinding<T> Bind<T>(T target) where T : class => throw new NotImplementedException();

        public ITargetedBinding<T> Bind<T>(T target, Action<Action> wrapper) where T : class => throw new NotImplementedException();

        public void Dispose() { }
    }

    private sealed record DummyCommand : ICommand;

    private interface IDummyStore : IStore
    {
        public int CommandsReceived { get; }
    }

    private sealed class DummyStore : AbstractStore, IDummyStore
    {
        public DummyStore(IEmitter emitter) : base(emitter)
        {
            Register<DummyCommand>(() => CommandsReceived += 1);
        }

        public int CommandsReceived { get; private set; } = 0;
    }
}
