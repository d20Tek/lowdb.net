namespace D20Tek.LowDb.Adapters;

/// <summary>
/// An asynchronous storage adapter that keeps the document in memory only, with no file or
/// external persistence. Data is lost when the adapter instance is discarded. Useful for
/// testing and transient scenarios.
/// </summary>
/// <typeparam name="T">The document type held in memory.</typeparam>
public class MemoryStorageAdapterAsync<T> : IStorageAdapterAsync<T> where T : class
{
    private T? _data = null;

    /// <inheritdoc/>
    public Task<T?> Read(CancellationToken token = default) => Task.FromResult(_data);

    /// <inheritdoc/>
    public Task Write(T data, CancellationToken token = default)
    {
        _data = data;
        return Task.CompletedTask;
    }
}
