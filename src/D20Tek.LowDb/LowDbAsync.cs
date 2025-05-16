﻿//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace D20Tek.LowDb;

public class LowDbAsync<T>
    where T : class, new()
{
    private readonly IStorageAdapterAsync<T> _storageAdapter;
    private T _data;
    private bool _isLoaded = false;

    public LowDbAsync(IStorageAdapterAsync<T> storageAdapter, T? data = null)
    {
        _storageAdapter = storageAdapter;
        _isLoaded = data != null;
        _data = data ?? new();
    }

    public async Task Read(CancellationToken token = default)
    {
        var data = await _storageAdapter.Read(token);
        _data = data ?? new T();
    }

    public async Task Write(CancellationToken token = default)
    {
        await _storageAdapter.Write(_data, token);
    }

    public async Task<T> Get(CancellationToken token = default)
    {
        await EnsureDatabaseLoaded(token);
        return _data;
    }

    public async Task Update(Action<T> updateAction, bool autoSave = true, CancellationToken token = default)
    {
        await EnsureDatabaseLoaded(token);
        updateAction(_data);

        if (autoSave is true)
        {
            await Write(token);
        }
    }

    private async Task EnsureDatabaseLoaded(CancellationToken token = default)
    {
        if (_isLoaded is false)
        {
            await Read(token);
            _isLoaded = true;
        }
    }
}
