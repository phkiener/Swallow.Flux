namespace Swallow.Flux.Default;

internal sealed class TargetedBinding<T> : ITargetedBinding<T>, IDisposable where T : class
{
    private readonly T target;
    private readonly Action<Action>? wrapper;
    private readonly IEmitter emitter;
    private readonly List<ISubscription> subscriptions = [];

    public TargetedBinding(T target, Action<Action>? wrapper, IEmitter emitter)
    {
        this.target = target;
        this.wrapper = wrapper;
        this.emitter = emitter;

        emitter.OnEmit += InvokeSubscribers;
    }

    public ITargetedBinding<T> To<TNotification>(Action<T> reaction, bool immediatelyInvoke = false) where TNotification : INotification
    {
        var subscription = new GeneralSubscription<TNotification>(reaction);
        subscriptions.Add(subscription);

        if (immediatelyInvoke)
        {
            // The notification gets discarded anyway, so we can just safely pass null here
            Invoke(subscription, null!);
        }

        return this;
    }

    public ITargetedBinding<T> To<TNotification>(Action<T, TNotification> reaction) where TNotification : INotification
    {
        var subscription = new Subscription<TNotification>(reaction);
        subscriptions.Add(subscription);

        return this;
    }

    public ITargetedBinding<T> To<TNotification>(Action<T, TNotification> reaction, bool immediatelyInvoke) where TNotification : INotification, new()
    {
        var subscription = new Subscription<TNotification>(reaction);
        subscriptions.Add(subscription);

        if (immediatelyInvoke)
        {
            Invoke(subscription, new TNotification());
        }

        return this;
    }

    public void Dispose()
    {
        emitter.OnEmit -= InvokeSubscribers;
    }

    private void InvokeSubscribers(object? sender, INotification notification)
    {
        foreach (var subscription in subscriptions.Where(s => s.CanHandle(notification)))
        {
            Invoke(subscription, notification);
        }
    }

    private void Invoke(ISubscription subscription, INotification notification)
    {
        if (wrapper is not null)
        {
            wrapper.Invoke(() => subscription.Handle(target, notification));
        }
        else
        {
            subscription.Handle(target, notification);
        }
    }

    private interface ISubscription
    {
        public bool CanHandle(object notification);

        public void Handle(T target, object notification);
    }

    private sealed class GeneralSubscription<TNotification>(Action<T> handler) : ISubscription
    {
        public bool CanHandle(object notification) => notification is TNotification;

        public void Handle(T target, object notification) => handler(target);
    }

    private sealed class Subscription<TNotification>(Action<T, TNotification> handler) : ISubscription
    {
        public bool CanHandle(object notification) => notification is TNotification;

        public void Handle(T target, object notification) => handler(target, (TNotification)notification);
    }
}
