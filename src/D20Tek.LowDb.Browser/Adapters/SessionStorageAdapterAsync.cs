using D20Tek.Blazor.BrowserStorage;

namespace D20Tek.LowDb.Browser.Adapters;

/// <summary>
/// An asynchronous storage adapter that persists a LowDb document to the browser's session
/// storage under a specified key. Session storage data is cleared when the browser tab or
/// session ends.
/// </summary>
/// <typeparam name="T">The document type stored in session storage.</typeparam>
/// <param name="keyname">The session storage key under which the document is stored.</param>
/// <param name="storage">The browser session storage service used to read and write values.</param>
public class SessionStorageAdapterAsync<T>(string keyname, ISessionStorageService storage) : IStorageAdapterAsync<T>
    where T : class
{
    private readonly string _keyname = keyname;
    private readonly ISessionStorageService _storage = storage;

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
