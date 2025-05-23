﻿//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;

namespace D20Tek.LowDb.Adapters;

public class JsonFileAdapterAsync<T> : IStorageAdapterAsync<T>
    where T : class
{
    private readonly string filename;
    private readonly TextFileAdapterAsync _textAdapter;

    private static JsonSerializerOptions _serializerOptions = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public JsonFileAdapterAsync(string filename)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(filename, nameof(filename));
        this.filename = filename;
        _textAdapter = new TextFileAdapterAsync(filename);
    }

    public async Task<T?> Read(CancellationToken token = default)
    {
        var json = await _textAdapter.Read(token);
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }
        else
        {
            var result = JsonSerializer.Deserialize<T>(json, _serializerOptions);
            return result;
        }
    }

    public async Task Write(T data, CancellationToken token = default)
    {
        var json = JsonSerializer.Serialize<T>(data, _serializerOptions);
        await _textAdapter.Write(json, token);
    }
}
