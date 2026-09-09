namespace D20Tek.LowDb.Adapters;

/// <summary>
/// A synchronous storage adapter that reads and writes raw text to a file, creating the
/// containing folder when necessary. Serves as the low-level file access layer used by
/// <see cref="JsonFileAdapter{T}"/>.
/// </summary>
/// <param name="filename">The path of the text file used for persistence.</param>
public class TextFileAdapter(string filename) : IStorageAdapter<string>
{
    private readonly string _filename = filename;

    /// <inheritdoc/>
    public string? Read()
    {
        EnsureFolderExists();
        return File.Exists(_filename) is false ? null : File.ReadAllText(_filename);
    }

    /// <inheritdoc/>
    public void Write(string data)
    {
        EnsureFolderExists();
        File.WriteAllText(_filename, data);
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
