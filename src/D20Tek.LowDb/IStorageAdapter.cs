namespace D20Tek.LowDb;

/// <summary>
/// Defines a synchronous storage backend that a <see cref="LowDb{T}"/> instance uses to
/// persist and retrieve its document. Implementations map the in-memory document to a
/// concrete store such as a JSON file, plain text file, or an in-memory buffer.
/// </summary>
/// <typeparam name="T">The reference type of the document that is persisted.</typeparam>
public interface IStorageAdapter<T>
    where T : class
{
    /// <summary>
    /// Reads the persisted document from the underlying store.
    /// </summary>
    /// <returns>
    /// The deserialized document, or <see langword="null"/> when the store does not yet
    /// contain any data.
    /// </returns>
    T? Read();

    /// <summary>
    /// Writes the supplied document to the underlying store, replacing any existing content.
    /// </summary>
    /// <param name="data">The document to persist.</param>
    void Write(T data);
}
