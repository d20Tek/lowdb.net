//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using Blazored.LocalStorage;

namespace D20Tek.LowDb.Browser.Adapters;

public class LocalStorageAdapterAsync<T> : IStorageAdapterAsync<T>
    where T : class
{
    private readonly string _keyname;
    private readonly ILocalStorageService _storage;

    public LocalStorageAdapterAsync(string keyname, ILocalStorageService storage)
    {
        _keyname = keyname;
        _storage = storage;
    }

    public async Task<T?> Read() => await _storage.GetItemAsync<T>(_keyname);

    public async Task Write(T data)
    {
        if (string.IsNullOrEmpty(_keyname)) throw new ArgumentException("keyname");
        await _storage.SetItemAsync<T>(_keyname, data);
    }
}
