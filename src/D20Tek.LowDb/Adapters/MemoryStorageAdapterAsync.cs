//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace D20Tek.LowDb.Adapters;

public class MemoryStorageAdapterAsync<T> : IStorageAdapterAsync<T>
    where T : class
{
    private T? _data = null;

    public Task<T?> Read(CancellationToken token = default)
    {
        return Task.FromResult(_data);
    }

    public Task Write(T data, CancellationToken token = default)
    {
        _data = data;
        return Task.CompletedTask;
    }
}
