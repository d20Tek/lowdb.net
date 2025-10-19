//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.LowDb.Adapters;

namespace D20Tek.LowDb;

public static class LowDbFactory
{
    public static LowDb<T> CreateJsonLowDb<T>(string filename)
        where T : class, new() =>
        new(new JsonFileAdapter<T>(filename));

    public static LowDb<T> CreateLowDb<T>(Action<LowDbBuilder> builderAction)
        where T : class, new()
    {
        var builder = new LowDbBuilder();
        builderAction(builder);

        return builder.Build<T>();
    }

    public static LowDbAsync<T> CreateJsonLowDbAsync<T>(string filename)
        where T : class, new() =>
        new(new JsonFileAdapterAsync<T>(filename));

    public static LowDbAsync<T> CreateLowDbAsync<T>(Action<LowDbBuilder> builderAction)
        where T : class, new()
    {
        var builder = new LowDbBuilder();
        builderAction(builder);

        return builder.BuildAsync<T>();
    }
}
