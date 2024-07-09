//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace D20Tek.LowDb;

public class LowDb<T>
    where T : class, new()
{
    private readonly IStorageAdapter<T> _storageAdapter;
    private T _data;
    private bool isLoaded = false;

    public LowDb(IStorageAdapter<T> storageAdapter, T? data = null)
    {
        _storageAdapter = storageAdapter;
        _data = data ?? new();
    }

    public void Read()
    {
        var data = _storageAdapter.Read();
        _data = data ?? new T();
    }

    public void Write()
    {
        _storageAdapter.Write(_data);
    }

    public T Get()
    {
        EnsureDatabaseLoaded();
        return _data;
    }

    public void Update(Action<T> updateAction)
    {
        EnsureDatabaseLoaded();
        updateAction(_data);
        Write();
    }

    private void EnsureDatabaseLoaded()
    {
        if (isLoaded is false)
        {
            Read();
            isLoaded = true;
        }
    }
}
