using System.Text.Json;

namespace D20Tek.LowDb.Adapters;

/// <summary>
/// A synchronous storage adapter that persists a document to a JSON file. The document is
/// serialized with camel-case property names and case-insensitive, trailing-comma-tolerant
/// deserialization.
/// </summary>
/// <typeparam name="T">The document type to serialize to and from JSON.</typeparam>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonFileAdapter{T}"/> class targeting the
    /// specified JSON file.
    /// </summary>
    /// <param name="filename">The path of the JSON file used for persistence.</param>
    /// <exception cref="ArgumentException"><paramref name="filename"/> is <see langword="null"/> or empty.</exception>
    public JsonFileAdapter(string filename)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(filename, nameof(filename));
        _filename = filename;
        _textAdapter = new TextFileAdapter(filename);
    }

    /// <inheritdoc/>
    public T? Read()
    {
        var json = _textAdapter.Read();
        if (string.IsNullOrEmpty(json)) return null;

        return JsonSerializer.Deserialize<T>(json, _serializerOptions);
    }

    /// <inheritdoc/>
    public void Write(T data)
    {
        var json = JsonSerializer.Serialize<T>(data, _serializerOptions);
        _textAdapter.Write(json);
    }
}
