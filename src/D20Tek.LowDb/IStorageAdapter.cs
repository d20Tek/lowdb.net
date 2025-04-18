﻿//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace D20Tek.LowDb;

public interface IStorageAdapter<T>
    where T : class
{
    T? Read();

    void Write(T data);
}
