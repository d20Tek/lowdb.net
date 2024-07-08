//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.LowDb.Adapters;

namespace D20Tek.LowDb;

public static class LowDbFactory
{
    public static LowDb<T> CreateJsonLowDb<T>(string filename)
        where T : class, new()
    {
        return new LowDb<T>(new JsonFileAdapter<T>(filename));
    }

    public static LowDbAsync<T> CreateJsonLowDbAsync<T>(string filename)
        where T : class, new()
    {
        return new LowDbAsync<T>(new JsonFileAdapterAsync<T>(filename));
    }
}
