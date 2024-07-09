//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace D20Tek.LowDb.Adapters;

public class TextFileAdapter : IStorageAdapter<string>
{
    private readonly string _filename;

    public TextFileAdapter(string filename)
    {
        _filename = filename;
    }

    public string? Read()
    {
        var folder = Path.GetDirectoryName(_filename);

        EnsureFolderExists(folder);
        if (File.Exists(_filename) is false)
        {
            return null;
        }

        var text = File.ReadAllText(_filename);
        return text;
    }

    public void Write(string data)
    {
        var folder = Path.GetDirectoryName(_filename);

        EnsureFolderExists(folder);
        File.WriteAllText(_filename, data);
    }

    public void EnsureFolderExists(string? folderPath)
    {
        if (string.IsNullOrEmpty(folderPath) is false)
        {
            Directory.CreateDirectory(folderPath);
        }
    }
}
