//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace D20Tek.LowDb.Adapters;

public class TextFileAdapterAsync : IFileAdapterAsync<string>
{
    private readonly string _filename;

    public TextFileAdapterAsync(string filename)
    {
        _filename = filename;
    }

    public async Task<string?> Read()
    {
        var folder = Path.GetDirectoryName(_filename);

        EnsureFolderExists(folder);
        if (File.Exists(_filename) is false)
        {
            return null;
        }

        var text = await File.ReadAllTextAsync(_filename);
        return text;
    }

    public async Task Write(string data)
    {
        var folder = Path.GetDirectoryName(_filename);

        EnsureFolderExists(folder);
        await File.WriteAllTextAsync(_filename, data);
    }

    public void EnsureFolderExists(string? folderPath)
    {
        if (string.IsNullOrEmpty(folderPath) is false)
        {
            Directory.CreateDirectory(folderPath);
        }
    }
}
