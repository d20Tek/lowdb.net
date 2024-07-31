//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace D20Tek.LowDb;

public class LowDb<T>
    where T : class, new()
{
    private readonly IStorageAdapter<T> _storageAdapter;
    private T _data;
    private bool _isLoaded = false;

    public LowDb(IStorageAdapter<T> storageAdapter, T? data = null)
    {
        _storageAdapter = storageAdapter;
        _isLoaded = data != null;
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

    public void Update(Action<T> updateAction, bool autoSave = true)
    {
        EnsureDatabaseLoaded();
        updateAction(_data);

        if (autoSave is true)
        {
            Write();
        }
    }

    private void EnsureDatabaseLoaded()
    {
        if (_isLoaded is false)
        {
            Read();
            _isLoaded = true;
        }
    }
}
