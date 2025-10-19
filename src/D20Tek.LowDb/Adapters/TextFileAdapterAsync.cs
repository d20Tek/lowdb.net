//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace D20Tek.LowDb.Adapters;

public class TextFileAdapterAsync(string filename) : IStorageAdapterAsync<string>
{
    private readonly string _filename = filename;

    public async Task<string?> Read(CancellationToken token = default)
    {
        EnsureFolderExists();
        return File.Exists(_filename) is false ? null : await File.ReadAllTextAsync(_filename, token);
    }

    public async Task Write(string data, CancellationToken token = default)
    {
        EnsureFolderExists();
        await File.WriteAllTextAsync(_filename, data, token);
    }

    public void EnsureFolderExists()
    {
        var folderPath = Path.GetDirectoryName(_filename);
        if (string.IsNullOrEmpty(folderPath) is false)
        {
            Directory.CreateDirectory(folderPath);
        }
    }
}
