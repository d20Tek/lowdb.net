using System.Text.Json;

namespace D20Tek.LowDb.Adapters;

/// <summary>
/// A synchronous storage adapter decorator that adds backup/recovery semantics to any
/// <see cref="IStorageAdapter{T}"/>. Before each write, the previous value read from the
/// wrapped primary adapter is written to a backup adapter. On read, if the primary adapter
/// returns <see langword="null"/> or throws a <see cref="JsonException"/>, the backup adapter
/// is consulted as a fallback.
/// </summary>
/// <typeparam name="T">The reference type of the document that is persisted.</typeparam>
/// <param name="primary">The primary storage adapter that is read from and written to first.</param>
/// <param name="backup">The storage adapter used to hold the previous good value.</param>
public class BackupStorageAdapter<T>(IStorageAdapter<T> primary, IStorageAdapter<T> backup) : IStorageAdapter<T>
    where T : class
{
    private readonly IStorageAdapter<T> _primary = primary;
    private readonly IStorageAdapter<T> _backup = backup;

    /// <inheritdoc/>
    public T? Read()
    {
        try
        {
            var result = _primary.Read();
            if (result is not null) return result;
        }
        catch (JsonException)
        {
            var backupResult = _backup.Read();
            if (backupResult is not null) return backupResult;

            throw;
        }

        return _backup.Read();
    }

    /// <inheritdoc/>
    public void Write(T data)
    {
        T? previous;
        try
        {
            previous = _primary.Read();
        }
        catch (JsonException)
        {
            // The primary value is corrupt; there is nothing valid to copy into the backup.
            previous = null;
        }

        if (previous is not null)
        {
            _backup.Write(previous);
        }

        _primary.Write(data);
    }
}
