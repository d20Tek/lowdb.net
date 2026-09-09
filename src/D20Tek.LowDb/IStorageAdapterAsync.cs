namespace D20Tek.LowDb;

/// <summary>
/// Defines an asynchronous storage backend that a <see cref="LowDbAsync{T}"/> instance uses
/// to persist and retrieve its document. Implementations map the in-memory document to a
/// concrete store such as a JSON file, an in-memory buffer, or browser web storage.
/// </summary>
/// <typeparam name="T">The reference type of the document that is persisted.</typeparam>
public interface IStorageAdapterAsync<T>
    where T : class
{
    /// <summary>
    /// Asynchronously reads the persisted document from the underlying store.
    /// </summary>
    /// <param name="token">A token used to observe cancellation requests.</param>
    /// <returns>
    /// A task whose result is the deserialized document, or <see langword="null"/> when the
    /// store does not yet contain any data.
    /// </returns>
    Task<T?> Read(CancellationToken token = default);

    /// <summary>
    /// Asynchronously writes the supplied document to the underlying store, replacing any
    /// existing content.
    /// </summary>
    /// <param name="data">The document to persist.</param>
    /// <param name="token">A token used to observe cancellation requests.</param>
    /// <returns>A task that completes when the write operation has finished.</returns>
    Task Write(T data, CancellationToken token = default);
}
