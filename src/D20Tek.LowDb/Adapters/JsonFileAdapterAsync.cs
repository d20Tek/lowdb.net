using System.Text.Json;

namespace D20Tek.LowDb.Adapters;

public class JsonFileAdapterAsync<T> : IStorageAdapterAsync<T> where T : class
{
    private readonly string _filename;
    private readonly TextFileAdapterAsync _textAdapter;

    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public JsonFileAdapterAsync(string filename)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(filename, nameof(filename));
        _filename = filename;
        _textAdapter = new TextFileAdapterAsync(filename);
    }

    public async Task<T?> Read(CancellationToken token = default)
    {
        var json = await _textAdapter.Read(token);
        if (string.IsNullOrEmpty(json)) return null;

        return JsonSerializer.Deserialize<T>(json, _serializerOptions);
    }

    public async Task Write(T data, CancellationToken token = default)
    {
        var json = JsonSerializer.Serialize<T>(data, _serializerOptions);
        await _textAdapter.Write(json, token);
    }
}
