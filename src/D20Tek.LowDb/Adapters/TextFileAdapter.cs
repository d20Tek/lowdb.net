//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace D20Tek.LowDb.Adapters;

public class TextFileAdapter(string filename) : IStorageAdapter<string>
{
    private readonly string _filename = filename;

    public string? Read()
    {
        EnsureFolderExists();
        return File.Exists(_filename) is false ? null : File.ReadAllText(_filename);
    }

    public void Write(string data)
    {
        EnsureFolderExists();
        File.WriteAllText(_filename, data);
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
