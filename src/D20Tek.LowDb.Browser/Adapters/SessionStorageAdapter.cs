//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using Blazored.SessionStorage;

namespace D20Tek.LowDb.Browser.Adapters;

public class SessionStorageAdapter<T> : IStorageAdapter<T>
    where T : class
{
    private readonly string _keyname;
    private readonly ISyncSessionStorageService _storage;

    public SessionStorageAdapter(string keyname, ISyncSessionStorageService storage)
    {
        _keyname = keyname;
        _storage = storage;
    }

    public T? Read() => _storage.GetItem<T>(_keyname);

    public void Write(T data)
    {
        if (string.IsNullOrEmpty(_keyname)) throw new ArgumentException("keyname");
        _storage.SetItem<T>(_keyname, data);
    }
}
