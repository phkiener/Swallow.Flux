namespace Swallow.Flux.Default;

public sealed partial class DefaultBinderTest
{
    [Test]
    public async Task InvokesBoundReaction_OnRelevantNotification()
    {
        var emitter = new DefaultEmitter();
        var binder = new DefaultBinder(emitter);

        string receivedText = "";
        binder.Bind().To<RelevantNotification>(() => receivedText += "received");

        emitter.Emit(new RelevantNotification());
        await Assert.That(receivedText).IsEqualTo("received");
    }

    [Test]
    public async Task DoesNotInvokeReaction_OnIrrelevantNotification()
    {
        var emitter = new DefaultEmitter();
        var binder = new DefaultBinder(emitter);

        string receivedText = "";
        binder.Bind().To<RelevantNotification>(() => receivedText += "received");

        emitter.Emit(new IrrelevantNotification());
        await Assert.That(receivedText).IsEmpty();
    }

    [Test]
    public async Task InvokesSubscriptionTwice_WhenBoundTwice()
    {
        var emitter = new DefaultEmitter();
        var binder = new DefaultBinder(emitter);

        string receivedText = "";
        binder.Bind()
            .To<RelevantNotification>(() => receivedText += "received")
            .To<RelevantNotification>(() => receivedText += "received");

        emitter.Emit(new RelevantNotification());
        await Assert.That(receivedText).IsEqualTo("received" + "received");
    }

    [Test]
    public async Task InvokesAllBoundSubscriptions()
    {
        var emitter = new DefaultEmitter();
        var binder = new DefaultBinder(emitter);

        string receivedText = "";
        binder.Bind().To<RelevantNotification>(() => receivedText += "a");
        binder.Bind().To<RelevantNotification>(() => receivedText += "b");

        emitter.Emit(new RelevantNotification());
        await Assert.That(receivedText).IsEqualTo("a" + "b");
    }

    [Test]
    public async Task InvokesWrapperAroundReaction()
    {
        var emitter = new DefaultEmitter();
        var binder = new DefaultBinder(emitter);

        string receivedText = "";
        binder.Bind(act => CatchAndWrite(act, ref receivedText)).To<RelevantNotification>(() => throw new InvalidOperationException("wrong!"));

        emitter.Emit(new RelevantNotification());
        await Assert.That(receivedText).IsEqualTo("wrong!");
    }

    [Test]
    public async Task InvokesReaction_WhenConfigured()
    {
        var emitter = new DefaultEmitter();
        var binder = new DefaultBinder(emitter);

        string receivedText = "";

        binder.Bind().To<RelevantNotification>(() => receivedText += "test", immediatelyInvoke: true);
        await Assert.That(receivedText).IsEqualTo("test");
    }

    [Test]
    public async Task PassesNotificationToReaction()
    {
        var emitter = new DefaultEmitter();
        var binder = new DefaultBinder(emitter);

        ParameterizedNotification? receivedNotification = null;

        binder.Bind().To<ParameterizedNotification>(n => receivedNotification = n);

        emitter.Emit(new ParameterizedNotification(Id: 42));
        await Assert.That(receivedNotification?.Id).IsEqualTo(42);
    }

    [Test]
    public async Task InvokesReactionWithDefaultConstructedNotification_WhenConfigured()
    {
        var emitter = new DefaultEmitter();
        var binder = new DefaultBinder(emitter);

        ParameterizedNotification? receivedNotification = null;

        binder.Bind().To<ParameterizedNotification>(n => receivedNotification = n, immediatelyInvoke: true);

        await Assert.That(receivedNotification?.Id).IsEqualTo(99);
    }

    [Test]
    public async Task InvokesWrapper_WhenImmediatelyInvoking()
    {
        var emitter = new DefaultEmitter();
        var binder = new DefaultBinder(emitter);

        int wrapperCalls = 0;

        binder.Bind(act => { wrapperCalls += 1; act.Invoke(); })
            .To<RelevantNotification>(() => { }, immediatelyInvoke: true)
            .To<RelevantNotification>(_ => { }, immediatelyInvoke: true);

        await Assert.That(wrapperCalls).IsEqualTo(2);
    }

    private static void CatchAndWrite(Action action, ref string output)
    {
        try
        {
            action.Invoke();
        }
        catch (Exception e)
        {
            output = e.Message;
        }
    }

    private sealed record RelevantNotification : INotification;
    private sealed record IrrelevantNotification : INotification;

    private sealed record ParameterizedNotification(int Id) : INotification
    {
        public ParameterizedNotification() : this(99) { }
    }
}
