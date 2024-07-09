//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace D20Tek.LowDb.Adapters;

public class MemoryStorageAdapterAsync<T> : IStorageAdapterAsync<T>
    where T : class
{
    private T? _data = null;

    public Task<T?> Read()
    {
        return Task.FromResult(_data);
    }

    public Task Write(T data)
    {
        _data = data;
        return Task.CompletedTask;
    }
}
