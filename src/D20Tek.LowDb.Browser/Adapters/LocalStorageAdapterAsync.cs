using D20Tek.Blazor.BrowserStorage;

namespace D20Tek.LowDb.Browser.Adapters;

/// <summary>
/// An asynchronous storage adapter that persists a LowDb document to the browser's local
/// storage under a specified key. Local storage data survives across browser sessions.
/// </summary>
/// <typeparam name="T">The document type stored in local storage.</typeparam>
/// <param name="keyname">The local storage key under which the document is stored.</param>
/// <param name="storage">The browser local storage service used to read and write values.</param>
public class LocalStorageAdapterAsync<T>(string keyname, ILocalStorageService storage) : IStorageAdapterAsync<T>
    where T : class
{
    private readonly string _keyname = keyname;
    private readonly ILocalStorageService _storage = storage;

    /// <inheritdoc/>
    public async Task<T?> Read(CancellationToken token = default)
    {
        var result = await _storage.GetAsync<T>(_keyname, token);
        return result.IsSuccess ? result.Value : default;
    }

    /// <inheritdoc/>
    public async Task Write(T data, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(_keyname, "keyname");
        await _storage.SetAsync(_keyname, data, token);
    }
}
