namespace D20Tek.LowDb;

public class LowDb<T>(IStorageAdapter<T> storageAdapter, T? data = null) where T : class, new()
{
    private readonly IStorageAdapter<T> _storageAdapter = storageAdapter;
    private readonly Lock _gate = new();
    private T _data = data ?? new();
    private bool _isLoaded = data != null;

    public void Read()
    {
        lock (_gate)
        {
            ReadCore();
        }
    }

    public void Write()
    {
        lock (_gate)
        {
            WriteCore();
        }
    }

    public T Get()
    {
        lock (_gate)
        {
            EnsureDatabaseLoaded();
            return _data;
        }
    }

    public void Update(Action<T> updateAction, bool autoSave = true)
    {
        lock (_gate)
        {
            EnsureDatabaseLoaded();
            updateAction(_data);

            if (autoSave is true)
            {
                WriteCore();
            }
        }
    }

    private void ReadCore() => _data = _storageAdapter.Read() ?? new T();

    private void WriteCore() => _storageAdapter.Write(_data);

    private void EnsureDatabaseLoaded()
    {
        if (_isLoaded is false)
        {
            ReadCore();
            _isLoaded = true;
        }
    }
}
