//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace D20Tek.LowDb.Adapters;

public class MemoryStorageAdapter<T> : IStorageAdapter<T>
    where T : class
{
    private T? _data = null;

    public T? Read()
    {
        return _data;
    }

    public void Write(T data)
    {
        _data = data;
    }
}
