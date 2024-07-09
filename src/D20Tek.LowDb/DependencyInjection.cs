//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.LowDb.Adapters;
using Microsoft.Extensions.DependencyInjection;

namespace D20Tek.LowDb;

public static class DependencyInjection
{
    public static IServiceCollection AddLowDb<T>(
        this IServiceCollection services,
        string filename,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where T : class, new()
    {
        ServiceDescriptor descriptor = new(
            typeof(LowDb<T>),
            sp => new LowDb<T>(new JsonFileAdapter<T>(filename)),
            lifetime);
        services.Add(descriptor);

        return services;
    }

    public static IServiceCollection AddLowDb<T>(
        this IServiceCollection services,
        Action<LowDbBuilder> builderAction)
        where T : class, new()
    {
        var builder = new LowDbBuilder();
        builderAction(builder);

        ServiceDescriptor descriptor = new(typeof(LowDb<T>), sp => builder.Build<T>(), builder.ServiceLifetime);
        services.Add(descriptor);
        return services;
    }

    public static IServiceCollection AddLowDbAsync<T>(
        this IServiceCollection services,
        string filename,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where T : class, new()
    {
        ServiceDescriptor descriptor = new(
            typeof(LowDbAsync<T>),
            sp => new LowDbAsync<T>(new JsonFileAdapterAsync<T>(filename)),
            lifetime);
        services.Add(descriptor);

        return services;
    }

    public static IServiceCollection AddLowDbAsync<T>(
        this IServiceCollection services,
        Action<LowDbBuilder> builderAction)
        where T : class, new()
    {
        var builder = new LowDbBuilder();
        builderAction(builder);

        ServiceDescriptor descriptor = new(
            typeof(LowDbAsync<T>),
            sp => builder.BuildAsync<T>(),
            builder.ServiceLifetime);

        services.Add(descriptor);
        return services;
    }
}
