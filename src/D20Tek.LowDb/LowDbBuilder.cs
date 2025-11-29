using D20Tek.LowDb.Adapters;
using Microsoft.Extensions.DependencyInjection;

namespace D20Tek.LowDb;

public class LowDbBuilder
{
    private string _filename = string.Empty;
    private string _folder = string.Empty;
    private bool _useMemoryAdapter = false;

    public ServiceLifetime ServiceLifetime { get; private set; } = ServiceLifetime.Singleton;

    public LowDbBuilder UseFileDatabase(string filename)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(filename, nameof(filename));
        _filename = filename;
        return this;
    }

    public LowDbBuilder UseInMemoryDatabase()
    {
        _useMemoryAdapter = true;
        return this;
    }

    public LowDbBuilder WithFolder(string folderName)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(folderName, nameof(folderName));
        _folder = folderName;
        return this;
    }

    public LowDbBuilder WithLifetime(ServiceLifetime serviceLifetime)
    {
        ServiceLifetime = serviceLifetime;
        return this;
    }

    public LowDb<T> Build<T>() where T : class, new()
    {
        IStorageAdapter<T> adapter;
        if (_useMemoryAdapter)
        {
            adapter = new MemoryStorageAdapter<T>();
        }
        else
        {
            ArgumentNullException.ThrowIfNullOrEmpty(_filename, nameof(_filename));
            string fullname = string.IsNullOrEmpty(_folder) ? _filename : Path.Combine(_folder, _filename);
            adapter = new JsonFileAdapter<T>(fullname);
        }

        return new(adapter);
    }

    public LowDbAsync<T> BuildAsync<T>() where T : class, new()
    {
        IStorageAdapterAsync<T> adapter;
        if (_useMemoryAdapter)
        {
            adapter = new MemoryStorageAdapterAsync<T>();
        }
        else
        {
            ArgumentNullException.ThrowIfNullOrEmpty(_filename, nameof(_filename));
            string fullname = string.IsNullOrEmpty(_folder) ? _filename : Path.Combine(_folder, _filename);
            adapter = new JsonFileAdapterAsync<T>(fullname);
        }

        return new(adapter);
    }
}
