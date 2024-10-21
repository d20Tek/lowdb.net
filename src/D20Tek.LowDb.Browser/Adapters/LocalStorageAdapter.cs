//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using Blazored.LocalStorage;

namespace D20Tek.LowDb.Browser.Adapters;

public class LocalStorageAdapter<T> : IStorageAdapter<T>
    where T : class
{
    private readonly string _keyname;
    private readonly ISyncLocalStorageService _storage;

    public LocalStorageAdapter(string keyname, ISyncLocalStorageService storage)
    {
        _keyname = keyname;
        _storage = storage;
    }

    public T? Read() => _storage.GetItem<T>(_keyname);

    public void Write(T data) => _storage.SetItem<T>(_keyname, data);
}
