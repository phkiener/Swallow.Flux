# Swallow.Flux

A tiny library containing an implementation of the Flux pattern.
I'm using it in two different projects, so I moved it to this shared library.

No big deal, honestly. Use at your own risk.

## Usage

Register the required services and all of your stores in your service collection:

```csharp
services.AddFlux()
    .AddStore<OrdersStore>()
    .AddStore<UnpaidBalanceStore>();
```

Each store can then react to commands, notifying all subscribers to changes:

```csharp
public sealed class UnpaidBalanceStore(IEmitter emitter) : IStore
{
    public decimal UnpaidBalance { get; private set; }

    public Task Handle(ICommand command, CancellationToken cancellationToken = default)
    {
        if (command is OrderPaid { PaidAmount: var paidAmount })
        {
            UnpaidBalance -= paidAmount;
            emitter.Emit<UnpaidBalanceChanged>();
        }

        // If we don't care about the command, we can just ignore it
        return Task.CompletedTask;
    }
}
```

Depending on the type of UI you're building, you can dispatch these commands and react
to the emitted events.

```csharp
public sealed class MyUiWindow : IDisposable
{
    private readonly IDispatcher dispatcher;
    private readonly IBinder binder;
    private readonly UnpaidBalanceStore unpaidBalanceStore;

    private readonly MyUiButton payButton = new UiButton();
    private readonly MyUiLabel unpaidBalanceLabel = new UiLabel();

    public MyUiWindow(
        IDispatcher dispatcher,
        IBinder binder,
        UnpaidBalanceStore unpaidBalanceStore)
    {
        this.dispatcher = dispatcher;
        this.binder = binder;
        this.unpaidBalanceStore unpaidBalanceStore binder;

        // Everytime the unpaid balance changes, we want to update the label to reflect that
        binder.Bind(unpaidBalanceLabel).To<UnpaidBalanceChanged>(UpdateUnpaidBalance)
    }

    private void UpdateButton(UiLabel label)
    {
        // We access the store to fetch the current value and display it to the user
        label.Text = $"Unpaid balance: {unpaidBalanceStore.UnpaidBalance}";
    }

    public async Task OnPayClicked(decimal amount)
    {
        // The user has paid - time to notify the stores of this fact
        await dispatcher.Dispatch(new OrderPaid(PaidAmount: amount));
    }

    public void Dispose()
    {
        // Unless overriden, the IBinder is registered as transient.
        // To properly dispose of all subscriptions, we should dispose the binder when disposing
        // the containing object.
        binder.Dipose();
    }
}
```
