using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace D20Tek.LowDb.Adapters;

/// <summary>
/// An asynchronous storage adapter decorator that adds backup/recovery semantics to any
/// <see cref="IStorageAdapterAsync{T}"/>. Before each write, the previous value read from the
/// wrapped primary adapter is written to a backup adapter. On read, if the primary adapter
/// returns <see langword="null"/> or throws a <see cref="JsonException"/>, the backup adapter
/// is consulted as a fallback.
/// </summary>
/// <typeparam name="T">The reference type of the document that is persisted.</typeparam>
/// <param name="primary">The primary storage adapter that is read from and written to first.</param>
/// <param name="backup">The storage adapter used to hold the previous good value.</param>
public class BackupStorageAdapterAsync<T>(IStorageAdapterAsync<T> primary, IStorageAdapterAsync<T> backup)
    : IStorageAdapterAsync<T>
    where T : class
{
    private readonly IStorageAdapterAsync<T> _primary = primary;
    private readonly IStorageAdapterAsync<T> _backup = backup;

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public async Task<T?> Read(CancellationToken token = default)
    {
        try
        {
            var result = await _primary.Read(token);
            if (result is not null) return result;
        }
        catch (JsonException)
        {
            var backupResult = await _backup.Read(token);
            if (backupResult is not null) return backupResult;

            throw;
        }

        return await _backup.Read(token);
    }

    /// <inheritdoc/>
    public async Task Write(T data, CancellationToken token = default)
    {
        T? previous;
        try
        {
            previous = await _primary.Read(token);
        }
        catch (JsonException)
        {
            // The primary value is corrupt; there is nothing valid to copy into the backup.
            previous = null;
        }

        if (previous is not null)
        {
            await _backup.Write(previous, token);
        }

        await _primary.Write(data, token);
    }
}
