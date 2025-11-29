using Blazored.SessionStorage;

namespace D20Tek.LowDb.Browser.Adapters;

public class SessionStorageAdapter<T>(string keyname, ISyncSessionStorageService storage) : IStorageAdapter<T>
    where T : class
{
    private readonly string _keyname = keyname;
    private readonly ISyncSessionStorageService _storage = storage;

    public T? Read() => _storage.GetItem<T>(_keyname);

    public void Write(T data)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(_keyname, "keyname");
        _storage.SetItem<T>(_keyname, data);
    }
}
