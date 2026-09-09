using D20Tek.LowDb.Adapters;

namespace D20Tek.LowDb;

/// <summary>
/// Provides factory methods for creating <see cref="LowDb{T}"/> and <see cref="LowDbAsync{T}"/>
/// instances, either directly from a JSON file name or through a fluent
/// <see cref="LowDbBuilder"/> configuration callback.
/// </summary>
public static class LowDbFactory
{
    /// <summary>
    /// Creates a synchronous JSON file-backed database using the specified file name.
    /// </summary>
    /// <typeparam name="T">The document type managed by the database.</typeparam>
    /// <param name="filename">The JSON database file name.</param>
    /// <returns>A configured <see cref="LowDb{T}"/> instance.</returns>
    public static LowDb<T> CreateJsonLowDb<T>(string filename)
        where T : class, new() =>
        new(new JsonFileAdapter<T>(filename));

    /// <summary>
    /// Creates a synchronous database configured through the supplied builder callback.
    /// </summary>
    /// <typeparam name="T">The document type managed by the database.</typeparam>
    /// <param name="builderAction">A callback that configures the <see cref="LowDbBuilder"/>.</param>
    /// <returns>A configured <see cref="LowDb{T}"/> instance.</returns>
    public static LowDb<T> CreateLowDb<T>(Action<LowDbBuilder> builderAction)
        where T : class, new()
    {
        var builder = new LowDbBuilder();
        builderAction(builder);

        return builder.Build<T>();
    }

    /// <summary>
    /// Creates an asynchronous JSON file-backed database using the specified file name.
    /// </summary>
    /// <typeparam name="T">The document type managed by the database.</typeparam>
    /// <param name="filename">The JSON database file name.</param>
    /// <returns>A configured <see cref="LowDbAsync{T}"/> instance.</returns>
    public static LowDbAsync<T> CreateJsonLowDbAsync<T>(string filename)
        where T : class, new() =>
        new(new JsonFileAdapterAsync<T>(filename));

    /// <summary>
    /// Creates an asynchronous database configured through the supplied builder callback.
    /// </summary>
    /// <typeparam name="T">The document type managed by the database.</typeparam>
    /// <param name="builderAction">A callback that configures the <see cref="LowDbBuilder"/>.</param>
    /// <returns>A configured <see cref="LowDbAsync{T}"/> instance.</returns>
    public static LowDbAsync<T> CreateLowDbAsync<T>(Action<LowDbBuilder> builderAction)
        where T : class, new()
    {
        var builder = new LowDbBuilder();
        builderAction(builder);

        return builder.BuildAsync<T>();
    }
}
