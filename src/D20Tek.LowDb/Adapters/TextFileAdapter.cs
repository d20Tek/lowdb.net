namespace D20Tek.LowDb.Adapters;

/// <summary>
/// A synchronous storage adapter that reads and writes raw text to a file, creating the
/// containing folder when necessary. Serves as the low-level file access layer used by
/// <see cref="JsonFileAdapter{T}"/>.
/// </summary>
/// <param name="filename">The path of the text file used for persistence.</param>
/// <param name="enableBackup">
/// When <see langword="true"/>, the previous file content is copied to a sibling
/// <c>.bak</c> file before each write, and <see cref="Read"/> falls back to that backup
/// when the primary file is missing.
/// </param>
public class TextFileAdapter(string filename, bool enableBackup = false) : IStorageAdapter<string>
{
    private readonly string _filename = filename;
    private readonly string _backupFilename = filename + ".bak";
    private readonly bool _enableBackup = enableBackup;

    /// <inheritdoc/>
    public string? Read()
    {
        EnsureFolderExists();

        if (File.Exists(_filename))
        {
            return File.ReadAllText(_filename);
        }

        if (_enableBackup && File.Exists(_backupFilename))
        {
            return File.ReadAllText(_backupFilename);
        }

        return null;
    }

    /// <inheritdoc/>
    public void Write(string data)
    {
        EnsureFolderExists();

        if (_enableBackup && File.Exists(_filename))
        {
            File.Copy(_filename, _backupFilename, overwrite: true);
        }

        File.WriteAllText(_filename, data);
    }

    /// <summary>
    /// Reads the backup file content directly, bypassing the primary file. Returns
    /// <see langword="null"/> when no backup file exists.
    /// </summary>
    public string? ReadBackup() => File.Exists(_backupFilename) ? File.ReadAllText(_backupFilename) : null;

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
