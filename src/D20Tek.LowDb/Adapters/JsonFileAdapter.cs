using System.Text.Json;

namespace D20Tek.LowDb.Adapters;

/// <summary>
/// A synchronous storage adapter that persists a document to a JSON file. By default, the
/// document is serialized with camel-case property names and case-insensitive,
/// trailing-comma-tolerant deserialization, but a custom <see cref="JsonSerializerOptions"/>
/// can be supplied to override this behavior.
/// </summary>
/// <typeparam name="T">The document type to serialize to and from JSON.</typeparam>
public class JsonFileAdapter<T> : IStorageAdapter<T> where T : class
{
    private readonly string _filename;
    private readonly TextFileAdapter _textAdapter;
    private readonly JsonSerializerOptions _serializerOptions;

    private static readonly JsonSerializerOptions DefaultSerializerOptions = new()
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
    /// <param name="serializerOptions">
    /// Optional <see cref="JsonSerializerOptions"/> used to serialize and deserialize the
    /// document. When <see langword="null"/>, defaults to camel-case property names with
    /// case-insensitive, trailing-comma-tolerant deserialization.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="filename"/> is <see langword="null"/> or empty.</exception>
    public JsonFileAdapter(string filename, JsonSerializerOptions? serializerOptions = null)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(filename, nameof(filename));
        _filename = filename;
        _textAdapter = new TextFileAdapter(filename);
        _serializerOptions = serializerOptions ?? DefaultSerializerOptions;
    }

    /// <inheritdoc/>
    public T? Read()
    {
        var json = _textAdapter.Read();
        return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<T>(json, _serializerOptions);
    }

    /// <inheritdoc/>
    public void Write(T data)
    {
        var json = JsonSerializer.Serialize<T>(data, _serializerOptions);
        _textAdapter.Write(json);
    }
}
