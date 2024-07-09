//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;

namespace D20Tek.LowDb.Adapters;

public class JsonFileAdapter<T> : IStorageAdapter<T>
    where T : class
{
    private readonly string filename;
    private readonly TextFileAdapter _textAdapter;

    private static JsonSerializerOptions _serializerOptions = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public JsonFileAdapter(string filename)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(filename, nameof(filename));
        this.filename = filename;
        _textAdapter = new TextFileAdapter(filename);
    }

    public T? Read()
    {
        var json = _textAdapter.Read();
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

    public void Write(T data)
    {
        var json = JsonSerializer.Serialize<T>(data, _serializerOptions);
        _textAdapter.Write(json);
    }
}
