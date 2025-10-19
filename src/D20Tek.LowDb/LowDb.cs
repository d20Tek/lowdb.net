//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace D20Tek.LowDb;

public class LowDb<T>(IStorageAdapter<T> storageAdapter, T? data = null)
    where T : class, new()
{
    private readonly IStorageAdapter<T> _storageAdapter = storageAdapter;
    private T _data = data ?? new();
    private bool _isLoaded = data != null;

    public void Read() => _data = _storageAdapter.Read() ?? new T();

    public void Write() => _storageAdapter.Write(_data);

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
