namespace D20Tek.LowDb.Adapters;

/// <summary>
/// A synchronous storage adapter that keeps the document in memory only, with no file or
/// external persistence. Data is lost when the adapter instance is discarded. Useful for
/// testing and transient scenarios.
/// </summary>
/// <typeparam name="T">The document type held in memory.</typeparam>
public class MemoryStorageAdapter<T> : IStorageAdapter<T> where T : class
{
    private T? _data = null;

    /// <inheritdoc/>
    public T? Read() => _data;

    /// <inheritdoc/>
    public void Write(T data) => _data = data;
}
