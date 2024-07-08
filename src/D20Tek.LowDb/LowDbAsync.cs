//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace D20Tek.LowDb;

public class LowDbAsync<T>
    where T : class, new()
{
    private readonly IFileAdapterAsync<T> _fileAdapter;
    private T _data;
    private bool isLoaded = false;

    public LowDbAsync(IFileAdapterAsync<T> fileAdapter, T? data = null)
    {
        _fileAdapter = fileAdapter;
        _data = data ?? new();
    }

    public async Task Read()
    {
        var data = await _fileAdapter.Read();
        _data = data ?? new T();
    }

    public async Task Write()
    {
        await _fileAdapter.Write(_data);
    }

    public async Task<T> Get()
    {
        await EnsureDatabaseLoaded();
        return _data;
    }

    public async Task Update(Action<T> updateAction)
    {
        await EnsureDatabaseLoaded();
        updateAction(_data);
        await Write();
    }

    private async Task EnsureDatabaseLoaded()
    {
        if (isLoaded is false)
        {
            await Read();
        }
    }
}
