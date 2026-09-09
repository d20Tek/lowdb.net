namespace D20Tek.LowDb.Adapters;

/// <summary>
/// An asynchronous storage adapter that reads and writes raw text to a file, creating the
/// containing folder when necessary. Serves as the low-level file access layer used by
/// <see cref="JsonFileAdapterAsync{T}"/>.
/// </summary>
/// <param name="filename">The path of the text file used for persistence.</param>
public class TextFileAdapterAsync(string filename) : IStorageAdapterAsync<string>
{
    private readonly string _filename = filename;

    /// <inheritdoc/>
    public async Task<string?> Read(CancellationToken token = default)
    {
        EnsureFolderExists();
        return File.Exists(_filename) is false ? null : await File.ReadAllTextAsync(_filename, token);
    }

    /// <inheritdoc/>
    public async Task Write(string data, CancellationToken token = default)
    {
        EnsureFolderExists();
        await File.WriteAllTextAsync(_filename, data, token);
    }

    /// <summary>
    /// Ensures the directory that contains the target file exists, creating it when needed.
    /// </summary>
    public void EnsureFolderExists()
    {
        var folderPath = Path.GetDirectoryName(_filename);
        if (string.IsNullOrEmpty(folderPath) is false)
        {
            Directory.CreateDirectory(folderPath);
        }
    }
}
