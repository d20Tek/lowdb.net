using System.Text.Json;

namespace D20Tek.LowDb.Adapters;

public class JsonFileAdapter<T> : IStorageAdapter<T> where T : class
{
    private readonly string _filename;
    private readonly TextFileAdapter _textAdapter;

    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public JsonFileAdapter(string filename)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(filename, nameof(filename));
        _filename = filename;
        _textAdapter = new TextFileAdapter(filename);
    }

    public T? Read()
    {
        var json = _textAdapter.Read();
        if (string.IsNullOrEmpty(json)) return null;

        return JsonSerializer.Deserialize<T>(json, _serializerOptions);
    }

    public void Write(T data)
    {
        var json = JsonSerializer.Serialize<T>(data, _serializerOptions);
        _textAdapter.Write(json);
    }
}
