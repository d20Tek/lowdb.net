using Blazored.LocalStorage;
using Blazored.SessionStorage;
using D20Tek.LowDb.Browser.Adapters;
using Microsoft.Extensions.DependencyInjection;

namespace D20Tek.LowDb.Browser;

public static class DependencyInjection
{
    public static IServiceCollection AddLocalLowDb<T>(
        this IServiceCollection services,
        string keyname,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where T : class, new()
    {
        services.RegisterBlazoredLocalStorage(lifetime);

        ServiceDescriptor descriptor = new(
            typeof(LowDb<T>),
            sp => new LowDb<T>(
                new LocalStorageAdapter<T>(keyname, sp.GetRequiredService<ISyncLocalStorageService>())),
                lifetime);
        services.Add(descriptor);

        return services;
    }

    public static IServiceCollection AddLocalLowDbAsync<T>(
        this IServiceCollection services,
        string keyname,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where T : class, new()
    {
        services.RegisterBlazoredLocalStorage(lifetime);

        ServiceDescriptor descriptor = new(
            typeof(LowDbAsync<T>),
            sp => new LowDbAsync<T>(
                new LocalStorageAdapterAsync<T>(keyname, sp.GetRequiredService<ILocalStorageService>())),
                lifetime);
        services.Add(descriptor);

        return services;
    }

    private static IServiceCollection RegisterBlazoredLocalStorage(this IServiceCollection s, ServiceLifetime l) =>
        (l != ServiceLifetime.Singleton) ? s.AddBlazoredLocalStorage() : s.AddBlazoredLocalStorageAsSingleton();

    public static IServiceCollection AddSessionLowDb<T>(
        this IServiceCollection services,
        string keyname,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where T : class, new()
    {
        services.RegisterBlazoredSessionStorage(lifetime);

        ServiceDescriptor descriptor = new(
            typeof(LowDb<T>),
            sp => new LowDb<T>(
                new SessionStorageAdapter<T>(keyname, sp.GetRequiredService<ISyncSessionStorageService>())),
                lifetime);
        services.Add(descriptor);

        return services;
    }

    public static IServiceCollection AddSessionLowDbAsync<T>(
        this IServiceCollection services,
        string keyname,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where T : class, new()
    {
        services.RegisterBlazoredSessionStorage(lifetime);

        ServiceDescriptor descriptor = new(
            typeof(LowDbAsync<T>),
            sp => new LowDbAsync<T>(
                new SessionStorageAdapterAsync<T>(keyname, sp.GetRequiredService<ISessionStorageService>())),
                lifetime);
        services.Add(descriptor);

        return services;
    }

    private static IServiceCollection RegisterBlazoredSessionStorage(this IServiceCollection s, ServiceLifetime l) =>
        (l != ServiceLifetime.Singleton) ? s.AddBlazoredSessionStorage() : s.AddBlazoredSessionStorageAsSingleton();
}