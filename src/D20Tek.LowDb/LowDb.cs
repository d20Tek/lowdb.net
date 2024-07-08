//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace D20Tek.LowDb;

public class LowDb<T>
    where T : class, new()
{
    private readonly IFileAdapter<T> _fileAdapter;
    private T _data;
    private bool isLoaded = false;

    public LowDb(IFileAdapter<T> fileAdapter, T? data = null)
    {
        _fileAdapter = fileAdapter;
        _data = data ?? new();
    }

    public void Read()
    {
        var data = _fileAdapter.Read();
        _data = data ?? new T();
    }

    public void Write()
    {
        _fileAdapter.Write(_data);
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
