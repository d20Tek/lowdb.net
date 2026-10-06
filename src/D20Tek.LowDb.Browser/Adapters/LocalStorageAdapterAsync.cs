using D20Tek.Blazor.BrowserStorage;

namespace D20Tek.LowDb.Browser.Adapters;

/// <summary>
/// An asynchronous storage adapter that persists a LowDb document to the browser's local
/// storage under a specified key. Local storage data survives across browser sessions.
/// </summary>
/// <typeparam name="T">The document type stored in local storage.</typeparam>
/// <param name="keyname">The local storage key under which the document is stored.</param>
/// <param name="storage">The browser local storage service used to read and write values.</param>
/// <param name="enableBackup">
/// When <see langword="true"/>, the previous value is copied to a sibling <c>.bak</c> key
/// before each write. <see cref="Read"/> falls back to that backup key when the primary key
/// is missing or its value fails to deserialize.
/// </param>
public class LocalStorageAdapterAsync<T>(string keyname, ILocalStorageService storage, bool enableBackup = false)
    : IStorageAdapterAsync<T>
    where T : class
{
    private readonly string _keyname = keyname;
    private readonly string _backupKeyname = keyname + ".bak";
    private readonly ILocalStorageService _storage = storage;
    private readonly bool _enableBackup = enableBackup;

    /// <inheritdoc/>
    public async Task<T?> Read(CancellationToken token = default)
    {
        var result = await _storage.GetAsync<T>(_keyname, token);
        if (result.IsSuccess) return result.Value;

        if (_enableBackup)
        {
            var backupResult = await _storage.GetAsync<T>(_backupKeyname, token);
            if (backupResult.IsSuccess) return backupResult.Value;
        }

        return default;
    }

    /// <inheritdoc/>
    public async Task Write(T data, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(_keyname, "keyname");

        if (_enableBackup)
        {
            var existing = await _storage.GetAsync<T>(_keyname, token);
            if (existing.IsSuccess && existing.Value is not null)
            {
                await _storage.SetAsync(_backupKeyname, existing.Value, token);
            }
        }

        await _storage.SetAsync(_keyname, data, token);
    }
}
