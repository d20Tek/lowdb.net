using D20Tek.Blazor.BrowserStorage;
using D20Tek.LowDb.Browser.Adapters;
using Microsoft.Extensions.DependencyInjection;

namespace D20Tek.LowDb.Browser;

public static class DependencyInjection
{
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