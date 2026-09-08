using D20Tek.Blazor.BrowserStorage;

namespace D20Tek.LowDb.Browser.Adapters;

public class SessionStorageAdapterAsync<T>(string keyname, ISessionStorageService storage) : IStorageAdapterAsync<T>
    where T : class
{
    private readonly string _keyname = keyname;
    private readonly ISessionStorageService _storage = storage;

    public async Task<T?> Read(CancellationToken token = default)
    {
        var result = await _storage.GetAsync<T>(_keyname, token);
        return result.IsSuccess ? result.Value : default;
    }

    public async Task Write(T data, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(_keyname, "keyname");
        await _storage.SetAsync(_keyname, data, token);
    }
}
