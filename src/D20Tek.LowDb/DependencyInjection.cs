using D20Tek.LowDb.Adapters;
using Microsoft.Extensions.DependencyInjection;

namespace D20Tek.LowDb;

/// <summary>
/// Provides extension methods for registering <see cref="LowDb{T}"/> and
/// <see cref="LowDbAsync{T}"/> instances with a dependency injection
/// <see cref="IServiceCollection"/>.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers a synchronous JSON file-backed <see cref="LowDb{T}"/> with the service collection.
    /// </summary>
    /// <typeparam name="T">The document type managed by the database.</typeparam>
    /// <param name="services">The service collection to add the registration to.</param>
    /// <param name="filename">The JSON database file name.</param>
    /// <param name="lifetime">The service lifetime for the registration. Defaults to <see cref="ServiceLifetime.Singleton"/>.</param>
    /// <returns>The same service collection so calls can be chained.</returns>
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

    /// <summary>
    /// Registers a synchronous <see cref="LowDb{T}"/> configured through a builder callback.
    /// The service lifetime is taken from the builder's <see cref="LowDbBuilder.ServiceLifetime"/>.
    /// </summary>
    /// <typeparam name="T">The document type managed by the database.</typeparam>
    /// <param name="services">The service collection to add the registration to.</param>
    /// <param name="builderAction">A callback that configures the <see cref="LowDbBuilder"/>.</param>
    /// <returns>The same service collection so calls can be chained.</returns>
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

    /// <summary>
    /// Registers an asynchronous JSON file-backed <see cref="LowDbAsync{T}"/> with the service collection.
    /// </summary>
    /// <typeparam name="T">The document type managed by the database.</typeparam>
    /// <param name="services">The service collection to add the registration to.</param>
    /// <param name="filename">The JSON database file name.</param>
    /// <param name="lifetime">The service lifetime for the registration. Defaults to <see cref="ServiceLifetime.Singleton"/>.</param>
    /// <returns>The same service collection so calls can be chained.</returns>
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

    /// <summary>
    /// Registers an asynchronous <see cref="LowDbAsync{T}"/> configured through a builder callback.
    /// The service lifetime is taken from the builder's <see cref="LowDbBuilder.ServiceLifetime"/>.
    /// </summary>
    /// <typeparam name="T">The document type managed by the database.</typeparam>
    /// <param name="services">The service collection to add the registration to.</param>
    /// <param name="builderAction">A callback that configures the <see cref="LowDbBuilder"/>.</param>
    /// <returns>The same service collection so calls can be chained.</returns>
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
