//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using Blazored.SessionStorage;

namespace D20Tek.LowDb.Browser.Adapters;

public class SessionStorageAdapterAsync<T> : IStorageAdapterAsync<T>
    where T : class
{
    private readonly string _keyname;
    private readonly ISessionStorageService _storage;

    public SessionStorageAdapterAsync(string keyname, ISessionStorageService storage)
    {
        _keyname = keyname;
        _storage = storage;
    }

    public async Task<T?> Read(CancellationToken token = default) => await _storage.GetItemAsync<T>(_keyname, token);

    public async Task Write(T data, CancellationToken token = default)
    {
        if (string.IsNullOrEmpty(_keyname)) throw new ArgumentException("keyname");
        await _storage.SetItemAsync<T>(_keyname, data, token);
    }
}
