//---------------------------------------------------------------------------------------------------------------------
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

    public async Task Read()
    {
        var data = await _storageAdapter.Read();
        _data = data ?? new T();
    }

    public async Task Write()
    {
        await _storageAdapter.Write(_data);
    }

    public async Task<T> Get()
    {
        await EnsureDatabaseLoaded();
        return _data;
    }

    public async Task Update(Action<T> updateAction, bool autoSave = true)
    {
        await EnsureDatabaseLoaded();
        updateAction(_data);

        if (autoSave is true)
        {
            await Write();
        }
    }

    private async Task EnsureDatabaseLoaded()
    {
        if (_isLoaded is false)
        {
            await Read();
        }
    }
}
