namespace D20Tek.LowDb;

/// <summary>
/// Provides a lightweight, asynchronous document database that keeps a single strongly typed
/// document in memory and persists it through an <see cref="IStorageAdapterAsync{T}"/>. Access
/// to the document is serialized with an internal <see cref="SemaphoreSlim"/> so that
/// concurrent callers on the same instance cannot overwrite one another's changes. Suitable
/// for file, in-memory, and browser web-storage backends.
/// </summary>
/// <typeparam name="T">
/// The document type managed by this database. It must be a reference type with a public
/// parameterless constructor so an empty document can be created when no data exists yet.
/// </typeparam>
/// <param name="storageAdapter">The asynchronous storage adapter used to read and write the document.</param>
/// <param name="data">
/// An optional pre-loaded document. When supplied, the database treats it as the current
/// state and skips the initial read from the storage adapter.
/// </param>
public class LowDbAsync<T>(IStorageAdapterAsync<T> storageAdapter, T? data = null) : IDisposable
    where T : class, new()
{
    private readonly IStorageAdapterAsync<T> _storageAdapter = storageAdapter;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private T _data = data ?? new();
    private bool _isLoaded = data != null;

    /// <summary>
    /// Asynchronously reads the document from the underlying storage adapter and replaces the
    /// in-memory state with the loaded value. When the store is empty, a new empty document is used.
    /// </summary>
    /// <param name="token">A token used to observe cancellation requests.</param>
    /// <returns>A task that completes when the read operation has finished.</returns>
    public async Task Read(CancellationToken token = default)
    {
        await _gate.WaitAsync(token);
        try
        {
            await ReadCore(token);
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>
    /// Asynchronously persists the current in-memory document to the underlying storage adapter.
    /// </summary>
    /// <param name="token">A token used to observe cancellation requests.</param>
    /// <returns>A task that completes when the write operation has finished.</returns>
    public async Task Write(CancellationToken token = default)
    {
        await _gate.WaitAsync(token);
        try
        {
            await WriteCore(token);
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>
    /// Asynchronously gets the current in-memory document, loading it from storage on first
    /// access if it has not been loaded yet.
    /// </summary>
    /// <param name="token">A token used to observe cancellation requests.</param>
    /// <returns>
    /// A task whose result is the live document instance. Mutations made to the returned object
    /// are reflected in the database state and can be persisted with <see cref="Write"/>.
    /// </returns>
    public async Task<T> Get(CancellationToken token = default)
    {
        await _gate.WaitAsync(token);
        try
        {
            await EnsureDatabaseLoaded(token);
            return _data;
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>
    /// Asynchronously applies a mutation to the current document and, by default, persists the
    /// result. The load, mutation, and optional save execute as a single serialized unit.
    /// </summary>
    /// <param name="updateAction">The action that mutates the document in place.</param>
    /// <param name="autoSave">
    /// When <see langword="true"/> (the default) the document is written to storage after the
    /// mutation completes; when <see langword="false"/> the change stays in memory until
    /// <see cref="Write"/> is called.
    /// </param>
    /// <param name="token">A token used to observe cancellation requests.</param>
    /// <returns>A task that completes when the update (and optional save) has finished.</returns>
    public async Task Update(Action<T> updateAction, bool autoSave = true, CancellationToken token = default)
    {
        await _gate.WaitAsync(token);
        try
        {
            await EnsureDatabaseLoaded(token);
            updateAction(_data);

            if (autoSave is true)
            {
                await WriteCore(token);
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>
    /// Releases the resources used by this instance, including the internal synchronization
    /// primitive that guards concurrent access.
    /// </summary>
    public void Dispose()
    {
        _gate.Dispose();
        GC.SuppressFinalize(this);
    }

    private async Task ReadCore(CancellationToken token = default) =>
        _data = await _storageAdapter.Read(token) ?? new T();

    private async Task WriteCore(CancellationToken token = default) =>
        await _storageAdapter.Write(_data, token);

    private async Task EnsureDatabaseLoaded(CancellationToken token = default)
    {
        if (_isLoaded is false)
        {
            await ReadCore(token);
            _isLoaded = true;
        }
    }
}
