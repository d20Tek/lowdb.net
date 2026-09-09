using System.Text.Json;

namespace D20Tek.LowDb.Adapters;

/// <summary>
/// An asynchronous storage adapter that persists a document to a JSON file. The document is
/// serialized with camel-case property names and case-insensitive, trailing-comma-tolerant
/// deserialization.
/// </summary>
/// <typeparam name="T">The document type to serialize to and from JSON.</typeparam>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonFileAdapterAsync{T}"/> class targeting
    /// the specified JSON file.
    /// </summary>
    /// <param name="filename">The path of the JSON file used for persistence.</param>
    /// <exception cref="ArgumentException"><paramref name="filename"/> is <see langword="null"/> or empty.</exception>
    public JsonFileAdapterAsync(string filename)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(filename, nameof(filename));
        _filename = filename;
        _textAdapter = new TextFileAdapterAsync(filename);
    }

    /// <inheritdoc/>
    public async Task<T?> Read(CancellationToken token = default)
    {
        var json = await _textAdapter.Read(token);
        if (string.IsNullOrEmpty(json)) return null;

        return JsonSerializer.Deserialize<T>(json, _serializerOptions);
    }

    /// <inheritdoc/>
    public async Task Write(T data, CancellationToken token = default)
    {
        var json = JsonSerializer.Serialize<T>(data, _serializerOptions);
        await _textAdapter.Write(json, token);
    }
}
