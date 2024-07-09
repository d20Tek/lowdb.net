//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace D20Tek.LowDb;

public interface IStorageAdapterAsync<T>
    where T : class
{
    Task<T?> Read();

    Task Write(T data);
}
