using D20Tek.LowDb.Adapters;
using Microsoft.Extensions.DependencyInjection;

namespace D20Tek.LowDb;

/// <summary>
/// Provides a fluent builder for configuring and creating <see cref="LowDb{T}"/> and
/// <see cref="LowDbAsync{T}"/> instances. Use the builder to select a storage backend
/// (file or in-memory), an optional folder, and the service lifetime used during
/// dependency injection registration.
/// </summary>
public class LowDbBuilder
{
    private string _filename = string.Empty;
    private string _folder = string.Empty;
    private bool _useMemoryAdapter = false;

    /// <summary>
    /// Gets the service lifetime that dependency injection registrations should use for the
    /// database built by this instance. Defaults to <see cref="ServiceLifetime.Singleton"/>.
    /// </summary>
    public ServiceLifetime ServiceLifetime { get; private set; } = ServiceLifetime.Singleton;

    /// <summary>
    /// Configures the database to persist to a JSON file with the specified name.
    /// </summary>
    /// <param name="filename">The database file name. Combined with the folder when one is set.</param>
    /// <returns>The same builder instance so calls can be chained.</returns>
    /// <exception cref="ArgumentException"><paramref name="filename"/> is <see langword="null"/> or empty.</exception>
    public LowDbBuilder UseFileDatabase(string filename)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(filename, nameof(filename));
        _filename = filename;
        return this;
    }

    /// <summary>
    /// Configures the database to store its document in memory only, with no file persistence.
    /// Useful for testing and transient scenarios.
    /// </summary>
    /// <returns>The same builder instance so calls can be chained.</returns>
    public LowDbBuilder UseInMemoryDatabase()
    {
        _useMemoryAdapter = true;
        return this;
    }

    /// <summary>
    /// Sets the folder in which the database file is stored. The folder is combined with the
    /// file name supplied to <see cref="UseFileDatabase"/>.
    /// </summary>
    /// <param name="folderName">The folder path that will contain the database file.</param>
    /// <returns>The same builder instance so calls can be chained.</returns>
    /// <exception cref="ArgumentException"><paramref name="folderName"/> is <see langword="null"/> or empty.</exception>
    public LowDbBuilder WithFolder(string folderName)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(folderName, nameof(folderName));
        _folder = folderName;
        return this;
    }

    /// <summary>
    /// Sets the service lifetime used when the database is registered with a dependency
    /// injection container.
    /// </summary>
    /// <param name="serviceLifetime">The desired service lifetime.</param>
    /// <returns>The same builder instance so calls can be chained.</returns>
    public LowDbBuilder WithLifetime(ServiceLifetime serviceLifetime)
    {
        ServiceLifetime = serviceLifetime;
        return this;
    }

    /// <summary>
    /// Builds a synchronous <see cref="LowDb{T}"/> instance using the configured storage backend.
    /// </summary>
    /// <typeparam name="T">The document type managed by the database.</typeparam>
    /// <returns>A configured <see cref="LowDb{T}"/> instance.</returns>
    /// <exception cref="ArgumentException">
    /// A file database was selected but no file name was provided.
    /// </exception>
    public LowDb<T> Build<T>() where T : class, new()
    {
        IStorageAdapter<T> adapter;
        if (_useMemoryAdapter)
        {
            adapter = new MemoryStorageAdapter<T>();
        }
        else
        {
            ArgumentNullException.ThrowIfNullOrEmpty(_filename, nameof(_filename));
            string fullname = string.IsNullOrEmpty(_folder) ? _filename : Path.Combine(_folder, _filename);
            adapter = new JsonFileAdapter<T>(fullname);
        }

        return new(adapter);
    }

    /// <summary>
    /// Builds an asynchronous <see cref="LowDbAsync{T}"/> instance using the configured storage backend.
    /// </summary>
    /// <typeparam name="T">The document type managed by the database.</typeparam>
    /// <returns>A configured <see cref="LowDbAsync{T}"/> instance.</returns>
    /// <exception cref="ArgumentException">
    /// A file database was selected but no file name was provided.
    /// </exception>
    public LowDbAsync<T> BuildAsync<T>() where T : class, new()
    {
        IStorageAdapterAsync<T> adapter;
        if (_useMemoryAdapter)
        {
            adapter = new MemoryStorageAdapterAsync<T>();
        }
        else
        {
            ArgumentNullException.ThrowIfNullOrEmpty(_filename, nameof(_filename));
            string fullname = string.IsNullOrEmpty(_folder) ? _filename : Path.Combine(_folder, _filename);
            adapter = new JsonFileAdapterAsync<T>(fullname);
        }

        return new(adapter);
    }
}
