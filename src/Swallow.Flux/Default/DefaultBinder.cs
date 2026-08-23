namespace Swallow.Flux.Default;

/// <summary>
/// A default binder that will keep all subscriptions until it is disposed
/// </summary>
/// <param name="emitter">The emitter to subscribe to</param>
public sealed class DefaultBinder(IEmitter emitter) : IBinder
{
    private readonly List<IDisposable> bindings = [];

    /// <inheritdoc />
    public ITargetedBinding<T> Bind<T>(T target) where T : class
    {
        var binding = new TargetedBinding<T>(target, null, emitter);
        bindings.Add(binding);

        return binding;
    }

    /// <inheritdoc />
    public ITargetedBinding<T> Bind<T>(T target, Action<Action> wrapper) where T : class
    {
        var binding = new TargetedBinding<T>(target, wrapper, emitter);
        bindings.Add(binding);

        return binding;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var binding in bindings)
        {
            binding.Dispose();
        }
    }
}
