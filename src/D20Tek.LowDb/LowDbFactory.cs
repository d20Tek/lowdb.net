using D20Tek.LowDb.Adapters;
using System.Text.Json;

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
    /// <param name="serializerOptions">
    /// Optional <see cref="JsonSerializerOptions"/> used to serialize and deserialize the
    /// document. When <see langword="null"/>, defaults to camel-case property names with
    /// case-insensitive, trailing-comma-tolerant deserialization.
    /// </param>
    /// <param name="enableBackup">
    /// When <see langword="true"/>, the previous file content is copied to a sibling
    /// <c>.bak</c> file before each write, and reads fall back to that backup when the
    /// primary file is missing or contains invalid JSON.
    /// </param>
    /// <returns>A configured <see cref="LowDb{T}"/> instance.</returns>
    public static LowDb<T> CreateJsonLowDb<T>(
        string filename,
        JsonSerializerOptions? serializerOptions = null,
        bool enableBackup = false)
        where T : class, new()
    {
        IStorageAdapter<T> adapter = new JsonFileAdapter<T>(filename, serializerOptions);
        if (enableBackup)
        {
            adapter = new BackupStorageAdapter<T>(adapter, new JsonFileAdapter<T>(filename + ".bak", serializerOptions));
        }

        return new(adapter);
    }

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
    /// <param name="serializerOptions">
    /// Optional <see cref="JsonSerializerOptions"/> used to serialize and deserialize the
    /// document. When <see langword="null"/>, defaults to camel-case property names with
    /// case-insensitive, trailing-comma-tolerant deserialization.
    /// </param>
    /// <param name="enableBackup">
    /// When <see langword="true"/>, the previous file content is copied to a sibling
    /// <c>.bak</c> file before each write, and reads fall back to that backup when the
    /// primary file is missing or contains invalid JSON.
    /// </param>
    /// <returns>A configured <see cref="LowDbAsync{T}"/> instance.</returns>
    public static LowDbAsync<T> CreateJsonLowDbAsync<T>(
        string filename,
        JsonSerializerOptions? serializerOptions = null,
        bool enableBackup = false)
        where T : class, new()
    {
        IStorageAdapterAsync<T> adapter = new JsonFileAdapterAsync<T>(filename, serializerOptions);
        if (enableBackup)
        {
            adapter = new BackupStorageAdapterAsync<T>(
                adapter, new JsonFileAdapterAsync<T>(filename + ".bak", serializerOptions));
        }

        return new(adapter);
    }

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
