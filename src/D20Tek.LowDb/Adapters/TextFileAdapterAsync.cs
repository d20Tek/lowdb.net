namespace D20Tek.LowDb.Adapters;

/// <summary>
/// An asynchronous storage adapter that reads and writes raw text to a file, creating the
/// containing folder when necessary. Serves as the low-level file access layer used by
/// <see cref="JsonFileAdapterAsync{T}"/>.
/// </summary>
/// <param name="filename">The path of the text file used for persistence.</param>
/// <param name="enableBackup">
/// When <see langword="true"/>, the previous file content is copied to a sibling
/// <c>.bak</c> file before each write, and <see cref="Read"/> falls back to that backup
/// when the primary file is missing.
/// </param>
public class TextFileAdapterAsync(string filename, bool enableBackup = false) : IStorageAdapterAsync<string>
{
    private readonly string _filename = filename;
    private readonly string _backupFilename = filename + ".bak";
    private readonly bool _enableBackup = enableBackup;

    /// <inheritdoc/>
    public async Task<string?> Read(CancellationToken token = default)
    {
        EnsureFolderExists();

        if (File.Exists(_filename))
        {
            return await File.ReadAllTextAsync(_filename, token);
        }

        if (_enableBackup && File.Exists(_backupFilename))
        {
            return await File.ReadAllTextAsync(_backupFilename, token);
        }

        return null;
    }

    /// <inheritdoc/>
    public async Task Write(string data, CancellationToken token = default)
    {
        EnsureFolderExists();

        if (_enableBackup && File.Exists(_filename))
        {
            File.Copy(_filename, _backupFilename, overwrite: true);
        }

        await File.WriteAllTextAsync(_filename, data, token);
    }

    /// <summary>
    /// Reads the backup file content directly, bypassing the primary file. Returns
    /// <see langword="null"/> when no backup file exists.
    /// </summary>
    public async Task<string?> ReadBackup(CancellationToken token = default) =>
        File.Exists(_backupFilename) ? await File.ReadAllTextAsync(_backupFilename, token) : null;

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
