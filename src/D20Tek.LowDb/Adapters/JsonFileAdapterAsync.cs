using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace D20Tek.LowDb.Adapters;

/// <summary>
/// An asynchronous storage adapter that persists a document to a JSON file. By default, the
/// document is serialized with camel-case property names and case-insensitive,
/// trailing-comma-tolerant deserialization, but a custom <see cref="JsonSerializerOptions"/>
/// can be supplied to override this behavior.
/// </summary>
/// <typeparam name="T">The document type to serialize to and from JSON.</typeparam>
public class JsonFileAdapterAsync<T> : IStorageAdapterAsync<T> where T : class
{
    private readonly string _filename;
    private readonly TextFileAdapterAsync _textAdapter;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly bool _enableBackup;

    private static readonly JsonSerializerOptions DefaultSerializerOptions = new()
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
    public JsonFileAdapterAsync(
        string filename,
        JsonSerializerOptions? serializerOptions = null,
        bool enableBackup = false)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(filename, nameof(filename));
        _filename = filename;
        _textAdapter = new TextFileAdapterAsync(filename, enableBackup);
        _serializerOptions = serializerOptions ?? DefaultSerializerOptions;
        _enableBackup = enableBackup;
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public async Task<T?> Read(CancellationToken token = default)
    {
        var json = await _textAdapter.Read(token);
        if (string.IsNullOrEmpty(json)) return null;

        try
        {
            return JsonSerializer.Deserialize<T>(json, _serializerOptions);
        }
        catch (JsonException) when (_enableBackup)
        {
            var backupJson = await _textAdapter.ReadBackup(token);
            if (string.IsNullOrEmpty(backupJson)) throw;

            return JsonSerializer.Deserialize<T>(backupJson, _serializerOptions);
        }
    }

    /// <inheritdoc/>
    public async Task Write(T data, CancellationToken token = default)
    {
        var json = JsonSerializer.Serialize<T>(data, _serializerOptions);
        await _textAdapter.Write(json, token);
    }
}
