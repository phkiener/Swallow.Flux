namespace Swallow.Flux.Default;

public sealed partial class DefaultBinderTest
{
    [Test]
    public async Task TargetedBinding_InvokesBoundReaction_OnRelevantNotification()
    {
        var emitter = new DefaultEmitter();
        var binder = new DefaultBinder(emitter);

        const string target = "test";
        string receivedText = "";
        binder.Bind(target).To<RelevantNotification>(t => receivedText += t);

        emitter.Emit(new RelevantNotification());
        await Assert.That(receivedText).IsEqualTo(target);
    }

    [Test]
    public async Task TargetedBinding_DoesNotInvokeReaction_OnIrrelevantNotification()
    {
        var emitter = new DefaultEmitter();
        var binder = new DefaultBinder(emitter);

        const string target = "test";
        string receivedText = "";
        binder.Bind(target).To<RelevantNotification>(t => receivedText += t);

        emitter.Emit(new IrrelevantNotification());
        await Assert.That(receivedText).IsEmpty();
    }

    [Test]
    public async Task TargetedBinding_InvokesSubscriptionTwice_WhenBoundTwice()
    {
        var emitter = new DefaultEmitter();
        var binder = new DefaultBinder(emitter);

        const string target = "test";
        string receivedText = "";
        binder.Bind(target)
            .To<RelevantNotification>(t => receivedText += t)
            .To<RelevantNotification>(t => receivedText += t);

        emitter.Emit(new RelevantNotification());
        await Assert.That(receivedText).IsEqualTo(target + target);
    }

    [Test]
    public async Task TargetedBinding_InvokesAllBoundSubscriptions()
    {
        var emitter = new DefaultEmitter();
        var binder = new DefaultBinder(emitter);

        const string target = "test";
        const string otherTarget = "TEST";
        string receivedText = "";
        binder.Bind(target).To<RelevantNotification>(t => receivedText += t);
        binder.Bind(otherTarget).To<RelevantNotification>(t => receivedText += t);

        emitter.Emit(new RelevantNotification());
        await Assert.That(receivedText).IsEqualTo(target + otherTarget);
    }

    [Test]
    public async Task TargetedBinding_InvokesWrapperAroundReaction()
    {
        var emitter = new DefaultEmitter();
        var binder = new DefaultBinder(emitter);

        const string target = "test";
        string receivedText = "";
        binder.Bind(target, act => CatchAndWrite(act, ref receivedText)).To<RelevantNotification>(_ => throw new InvalidOperationException("wrong!"));

        emitter.Emit(new RelevantNotification());
        await Assert.That(receivedText).IsEqualTo("wrong!");
    }

    [Test]
    public async Task TargetedBinding_InvokesReaction_WhenConfigured()
    {
        var emitter = new DefaultEmitter();
        var binder = new DefaultBinder(emitter);

        const string target = "test";
        string receivedText = "";

        binder.Bind(target).To<RelevantNotification>(t => receivedText += t, immediatelyInvoke: true);
        await Assert.That(receivedText).IsEqualTo("test");
    }

    [Test]
    public async Task TargetedBinding_PassesNotificationToReaction()
    {
        var emitter = new DefaultEmitter();
        var binder = new DefaultBinder(emitter);

        const string target = "test";
        ParameterizedNotification? receivedNotification = null;

        binder.Bind(target).To<ParameterizedNotification>((_, n) => receivedNotification = n);

        emitter.Emit(new ParameterizedNotification(Id: 42));
        await Assert.That(receivedNotification?.Id).IsEqualTo(42);
    }

    [Test]
    public async Task TargetedBinding_InvokesReactionWithDefaultConstructedNotification_WhenConfigured()
    {
        var emitter = new DefaultEmitter();
        var binder = new DefaultBinder(emitter);

        const string target = "test";
        ParameterizedNotification? receivedNotification = null;

        binder.Bind(target).To<ParameterizedNotification>((_, n) => receivedNotification = n, immediatelyInvoke: true);

        await Assert.That(receivedNotification?.Id).IsEqualTo(99);
    }

    [Test]
    public async Task TargetedBinding_InvokesWrapper_WhenImmediatelyInvoking()
    {
        var emitter = new DefaultEmitter();
        var binder = new DefaultBinder(emitter);

        const string target = "test";
        int wrapperCalls = 0;

        binder.Bind(target, act => { wrapperCalls += 1; act.Invoke(); })
            .To<RelevantNotification>(_ => { }, immediatelyInvoke: true)
            .To<RelevantNotification>((_, _) => { }, immediatelyInvoke: true);

        await Assert.That(wrapperCalls).IsEqualTo(2);
    }
}
