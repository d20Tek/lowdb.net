using D20Tek.Blazor.BrowserStorage;
using D20Tek.LowDb.Browser.Adapters;
using Microsoft.Extensions.DependencyInjection;

namespace D20Tek.LowDb.Browser;

/// <summary>
/// Provides extension methods for registering browser-backed <see cref="LowDbAsync{T}"/>
/// instances that persist to local or session storage with a dependency injection
/// <see cref="IServiceCollection"/>.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers an asynchronous LowDb database backed by the browser's local storage.
    /// Also registers the required local storage services.
    /// </summary>
    /// <typeparam name="T">The document type managed by the database.</typeparam>
    /// <param name="services">The service collection to add the registration to.</param>
    /// <param name="keyname">The local storage key under which the document is stored.</param>
    /// <param name="lifetime">The service lifetime for the registration. Defaults to <see cref="ServiceLifetime.Scoped"/>.</param>
    /// <returns>The same service collection so calls can be chained.</returns>
    public static IServiceCollection AddLocalLowDbAsync<T>(
        this IServiceCollection services,
        string keyname,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where T : class, new()
    {
        services.AddLocalStorage(lifetime: lifetime);

        ServiceDescriptor descriptor = new(
            typeof(LowDbAsync<T>),
            sp => new LowDbAsync<T>(
                new LocalStorageAdapterAsync<T>(keyname, sp.GetRequiredService<ILocalStorageService>())),
                lifetime);
        services.Add(descriptor);

        return services;
    }

    /// <summary>
    /// Registers an asynchronous LowDb database backed by the browser's session storage.
    /// Also registers the required session storage services.
    /// </summary>
    /// <typeparam name="T">The document type managed by the database.</typeparam>
    /// <param name="services">The service collection to add the registration to.</param>
    /// <param name="keyname">The session storage key under which the document is stored.</param>
    /// <param name="lifetime">The service lifetime for the registration. Defaults to <see cref="ServiceLifetime.Scoped"/>.</param>
    /// <returns>The same service collection so calls can be chained.</returns>
    public static IServiceCollection AddSessionLowDbAsync<T>(
        this IServiceCollection services,
        string keyname,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where T : class, new()
    {
        services.AddSessionStorage(lifetime: lifetime);

        ServiceDescriptor descriptor = new(
            typeof(LowDbAsync<T>),
            sp => new LowDbAsync<T>(
                new SessionStorageAdapterAsync<T>(keyname, sp.GetRequiredService<ISessionStorageService>())),
                lifetime);
        services.Add(descriptor);

        return services;
    }
}