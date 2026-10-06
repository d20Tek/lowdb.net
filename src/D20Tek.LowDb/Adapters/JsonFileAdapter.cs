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
    private readonly bool _enableBackup;

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
    /// <param name="enableBackup">
    /// When <see langword="true"/>, the previous file content is copied to a sibling
    /// <c>.bak</c> file before each write. <see cref="Read"/> falls back to that backup
    /// when the primary file is missing or contains invalid JSON.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="filename"/> is <see langword="null"/> or empty.</exception>
    public JsonFileAdapter(
        string filename,
        JsonSerializerOptions? serializerOptions = null,
        bool enableBackup = false)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(filename, nameof(filename));
        _filename = filename;
        _textAdapter = new TextFileAdapter(filename, enableBackup);
        _serializerOptions = serializerOptions ?? DefaultSerializerOptions;
        _enableBackup = enableBackup;
    }

    /// <inheritdoc/>
    public T? Read()
    {
        var json = _textAdapter.Read();
        if (string.IsNullOrEmpty(json)) return null;

        try
        {
            return JsonSerializer.Deserialize<T>(json, _serializerOptions);
        }
        catch (JsonException) when (_enableBackup)
        {
            var backupJson = _textAdapter.ReadBackup();
            if (string.IsNullOrEmpty(backupJson)) throw;

            return JsonSerializer.Deserialize<T>(backupJson, _serializerOptions);
        }
    }

    /// <inheritdoc/>
    public void Write(T data)
    {
        var json = JsonSerializer.Serialize<T>(data, _serializerOptions);
        _textAdapter.Write(json);
    }
}
