namespace D20Tek.LowDb;

/// <summary>
/// Provides a lightweight, synchronous, file-backed document database that keeps a single
/// strongly typed document in memory and persists it through an <see cref="IStorageAdapter{T}"/>.
/// Access to the document is serialized with an internal lock so that concurrent callers on
/// the same instance cannot overwrite one another's changes.
/// </summary>
/// <typeparam name="T">
/// The document type managed by this database. It must be a reference type with a public
/// parameterless constructor so an empty document can be created when no data exists yet.
/// </typeparam>
/// <param name="storageAdapter">The storage adapter used to read and write the document.</param>
/// <param name="data">
/// An optional pre-loaded document. When supplied, the database treats it as the current
/// state and skips the initial read from the storage adapter.
/// </param>
public class LowDb<T>(IStorageAdapter<T> storageAdapter, T? data = null) where T : class, new()
{
    private readonly IStorageAdapter<T> _storageAdapter = storageAdapter;
    private readonly Lock _gate = new();
    private T _data = data ?? new();
    private bool _isLoaded = data != null;

    /// <summary>
    /// Reads the document from the underlying storage adapter and replaces the in-memory
    /// state with the loaded value. When the store is empty, a new empty document is used.
    /// </summary>
    public void Read()
    {
        lock (_gate)
        {
            ReadCore();
        }
    }

    /// <summary>
    /// Persists the current in-memory document to the underlying storage adapter.
    /// </summary>
    public void Write()
    {
        lock (_gate)
        {
            WriteCore();
        }
    }

    /// <summary>
    /// Gets the current in-memory document, loading it from storage on first access if it
    /// has not been loaded yet.
    /// </summary>
    /// <returns>
    /// The live document instance. Mutations made to the returned object are reflected in the
    /// database state and can be persisted with <see cref="Write"/>.
    /// </returns>
    public T Get()
    {
        lock (_gate)
        {
            EnsureDatabaseLoaded();
            return _data;
        }
    }

    /// <summary>
    /// Applies a mutation to the current document and, by default, persists the result.
    /// The load, mutation, and optional save execute as a single serialized unit.
    /// </summary>
    /// <param name="updateAction">The action that mutates the document in place.</param>
    /// <param name="autoSave">
    /// When <see langword="true"/> (the default) the document is written to storage after the
    /// mutation completes; when <see langword="false"/> the change stays in memory until
    /// <see cref="Write"/> is called.
    /// </param>
    public void Update(Action<T> updateAction, bool autoSave = true)
    {
        lock (_gate)
        {
            EnsureDatabaseLoaded();
            updateAction(_data);

            if (autoSave is true)
            {
                WriteCore();
            }
        }
    }

    private void ReadCore() => _data = _storageAdapter.Read() ?? new T();

    private void WriteCore() => _storageAdapter.Write(_data);

    private void EnsureDatabaseLoaded()
    {
        if (_isLoaded is false)
        {
            ReadCore();
            _isLoaded = true;
        }
    }
}
