//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.LowDb.Adapters;
using Microsoft.Extensions.DependencyInjection;

namespace D20Tek.LowDb;

public static class DependencyInjection
{
    public static IServiceCollection AddLowDb<T>(this IServiceCollection services, string filename)
        where T : class, new()
    {
        services.AddScoped(typeof(LowDb<T>), sp => new LowDb<T>(new JsonFileAdapter<T>(filename)));
        return services;
    }
}
