using D20Tek.Blazor.BrowserStorage;
using D20Tek.LowDb.Adapters;
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
    /// <param name="configureStorage">
    /// Optional configuration for the underlying local storage, including
    /// <see cref="BrowserStorageOptions.JsonOptions"/> used to serialize and deserialize the document.
    /// </param>
    /// <param name="enableBackup">
    /// When <see langword="true"/>, the previous value is copied to a sibling <c>.bak</c> key
    /// before each write, and reads fall back to that backup key when the primary key is
    /// missing or its value fails to deserialize.
    /// </param>
    /// <returns>The same service collection so calls can be chained.</returns>
    public static IServiceCollection AddLocalLowDbAsync<T>(
        this IServiceCollection services,
        string keyname,
        ServiceLifetime lifetime = ServiceLifetime.Scoped,
        Action<BrowserStorageOptions>? configureStorage = null,
        bool enableBackup = false)
        where T : class, new()
    {
        services.AddLocalStorage(configureStorage, lifetime);

        ServiceDescriptor descriptor = new(
            typeof(LowDbAsync<T>),
            sp =>
            {
                var localStorage = sp.GetRequiredService<ILocalStorageService>();
                IStorageAdapterAsync<T> adapter = new LocalStorageAdapterAsync<T>(keyname, localStorage);
                if (enableBackup)
                {
                    adapter = new BackupStorageAdapterAsync<T>(
                        adapter, new LocalStorageAdapterAsync<T>(keyname + ".bak", localStorage));
                }

                return new LowDbAsync<T>(adapter);
            },
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
    /// <param name="configureStorage">
    /// Optional configuration for the underlying session storage, including
    /// <see cref="BrowserStorageOptions.JsonOptions"/> used to serialize and deserialize the document.
    /// </param>
    /// <param name="enableBackup">
    /// When <see langword="true"/>, the previous value is copied to a sibling <c>.bak</c> key
    /// before each write, and reads fall back to that backup key when the primary key is
    /// missing or its value fails to deserialize.
    /// </param>
    /// <returns>The same service collection so calls can be chained.</returns>
    public static IServiceCollection AddSessionLowDbAsync<T>(
        this IServiceCollection services,
        string keyname,
        ServiceLifetime lifetime = ServiceLifetime.Scoped,
        Action<BrowserStorageOptions>? configureStorage = null,
        bool enableBackup = false)
        where T : class, new()
    {
        services.AddSessionStorage(configureStorage, lifetime);

        ServiceDescriptor descriptor = new(
            typeof(LowDbAsync<T>),
            sp =>
            {
                var sessionStorage = sp.GetRequiredService<ISessionStorageService>();
                IStorageAdapterAsync<T> adapter = new SessionStorageAdapterAsync<T>(keyname, sessionStorage);
                if (enableBackup)
                {
                    adapter = new BackupStorageAdapterAsync<T>(
                        adapter, new SessionStorageAdapterAsync<T>(keyname + ".bak", sessionStorage));
                }

                return new LowDbAsync<T>(adapter);
            },
            lifetime);
        services.Add(descriptor);

        return services;
    }
}