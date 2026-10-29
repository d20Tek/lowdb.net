namespace D20Tek.LowDb;

/// <summary>
/// Provides data for the change notification raised after a LowDb document has been
/// successfully persisted to its underlying storage adapter.
/// </summary>
/// <typeparam name="T">The document type that was persisted.</typeparam>
/// <param name="document">The document instance that was written to storage.</param>
public class DocumentChangedEventArgs<T>(T document) : EventArgs
    where T : class
{
    /// <summary>
    /// Gets the document instance that was written to storage. This is the live in-memory
    /// document, so handlers should treat it as read-only to avoid mutating database state
    /// outside of a normal update operation.
    /// </summary>
    public T Document { get; } = document;
}
