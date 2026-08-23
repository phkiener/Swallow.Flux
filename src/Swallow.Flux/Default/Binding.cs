namespace Swallow.Flux.Default;

internal sealed class Binding : IBinding, IDisposable
{
    private readonly Action<Action>? wrapper;
    private readonly IEmitter emitter;
    private readonly List<ISubscription> subscriptions = [];

    public Binding(Action<Action>? wrapper, IEmitter emitter)
    {
        this.wrapper = wrapper;
        this.emitter = emitter;

        emitter.OnEmit += InvokeSubscribers;
    }
    public IBinding To<TNotification>(Action reaction, bool immediatelyInvoke = false) where TNotification : INotification
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

    public IBinding To<TNotification>(Action<TNotification> reaction) where TNotification : INotification
    {
        var subscription = new Subscription<TNotification>(reaction);
        subscriptions.Add(subscription);

        return this;
    }

    public IBinding To<TNotification>(Action<TNotification> reaction, bool immediatelyInvoke) where TNotification : INotification, new()
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
            wrapper.Invoke(() => subscription.Handle(notification));
        }
        else
        {
            subscription.Handle(notification);
        }
    }

    private interface ISubscription
    {
        public bool CanHandle(object notification);

        public void Handle(object notification);
    }

    private sealed class GeneralSubscription<TNotification>(Action handler) : ISubscription
    {
        public bool CanHandle(object notification) => notification is TNotification;

        public void Handle(object notification) => handler();
    }

    private sealed class Subscription<TNotification>(Action<TNotification> handler) : ISubscription
    {
        public bool CanHandle(object notification) => notification is TNotification;

        public void Handle(object notification) => handler((TNotification)notification);
    }
}
