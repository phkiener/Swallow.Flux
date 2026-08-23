namespace Swallow.Flux.Default;

public sealed class DefaultEmitterTest
{
    [Test]
    public async Task InvokesEventHandler()
    {
        INotification? receivedNotification = null;
        object? receivedSender = null;

        var emitter = new DefaultEmitter();
        emitter.OnEmit += (sender, notification) =>
        {
            receivedSender = sender;
            receivedNotification = notification;
        };

        emitter.Emit(new DummyNotification());

        await Assert.That(receivedSender).IsSameReferenceAs(emitter);
        await Assert.That(receivedNotification).IsTypeOf<DummyNotification>();
    }

    private sealed record DummyNotification : INotification;
}
