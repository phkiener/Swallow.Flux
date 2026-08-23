using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Swallow.Flux.Default;

namespace Swallow.Flux;

/// <summary>
/// Extensions to register the framework and all your <see cref="IStore"/>s in a <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register all required Flux services with their default implementation
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> on which to register the services</param>
    /// <returns>The given <see cref="IServiceCollection"/> with additional registrations for the Flux services</returns>
    /// <remarks>
    /// The <see cref="IBinder"/> is registered as <em>transient</em> by default, which means that each object will get its very own instance of
    /// a binder. To properly dispose of the subscriptions to notifications, dispose that binder when disposing your object.
    /// </remarks>
    public static IServiceCollection AddFlux(this IServiceCollection services)
    {
        services.TryAddScoped<IDispatcher, DefaultDispatcher>();
        services.TryAddScoped<IEmitter, DefaultEmitter>();
        services.TryAddTransient<IBinder, DefaultBinder>();

        return services;
    }

    /// <summary>
    /// Register an <see cref="IStore"/> so that an <see cref="IDispatcher"/> may pick it up
    /// </summary>
    /// <typeparam name="TStore">Type of store to register</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> on which to register the store</param>
    /// <returns>The given <see cref="IServiceCollection"/> with additional registrations for the store</returns>
    public static IServiceCollection AddStore<TStore>(this IServiceCollection services) where TStore : class, IStore
    {
        services.AddScoped<IStore>(static sp => sp.GetRequiredService<TStore>());
        services.TryAddScoped<TStore>();

        return services;
    }

    /// <summary>
    /// Register an <see cref="IStore"/> so that an <see cref="IDispatcher"/> may pick it up
    /// </summary>
    /// <typeparam name="TStore">Type of store to register</typeparam>
    /// <typeparam name="TImplementation">Type implementing <typeparamref name="TStore"/> which will be registered as implementation</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> on which to register the store</param>
    /// <returns>The given <see cref="IServiceCollection"/> with additional registrations for the store</returns>
    /// <remarks>
    ///     <typeparamref name="TImplementation"/> is registered as well in addition to <typeparamref name="TStore"/> and <see cref="IStore"/>.
    ///     All of these will share the same (scoped) instance.
    /// </remarks>
    public static IServiceCollection AddStore<TStore, TImplementation>(this IServiceCollection services)
        where TStore : class
        where TImplementation : class, IStore, TStore
    {
        services.TryAddScoped<TImplementation>();
        services.TryAddScoped<TStore>(Resolve);
        services.AddScoped<IStore>(Resolve);

        return services;

        static TImplementation Resolve(IServiceProvider sp) => sp.GetRequiredService<TImplementation>();
    }
}
