using Blazored.LocalStorage;

namespace D20Tek.LowDb.Browser.Adapters;

public class LocalStorageAdapterAsync<T>(string keyname, ILocalStorageService storage) : IStorageAdapterAsync<T>
    where T : class
{
    private readonly string _keyname = keyname;
    private readonly ILocalStorageService _storage = storage;

    public async Task<T?> Read(CancellationToken token = default) => await _storage.GetItemAsync<T>(_keyname, token);

    public async Task Write(T data, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(_keyname, "keyname");
        await _storage.SetItemAsync<T>(_keyname, data, token);
    }
}
