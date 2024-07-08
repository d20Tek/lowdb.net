//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace D20Tek.LowDb;

public class LowDb<T>
    where T : class, new()
{
    private readonly IFileAdapter<T> _fileAdapter;
    private T? _data;

    public LowDb(IFileAdapter<T> fileAdapter, T? data = null)
    {
        _fileAdapter = fileAdapter;
        _data = data;
    }

    public void Read()
    {
        var data = _fileAdapter.Read();
        _data = data;
    }

    public void Write()
    {
        _fileAdapter.Write(_data ?? new());
    }

    public T? Get()
    {
        if (_data is null)
        {
            Read();
        }

        return _data;
    }

    public void Update(Action<T> updateAction)
    {
        if (_data is null)
        {
            Read();
            if (_data is null)
            {
                _data = new T();
            }
        }

        updateAction(_data);
        Write();
    }
}
