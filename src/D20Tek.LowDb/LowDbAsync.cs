namespace D20Tek.LowDb;

public class LowDbAsync<T>(IStorageAdapterAsync<T> storageAdapter, T? data = null) : IDisposable
    where T : class, new()
{
    private readonly IStorageAdapterAsync<T> _storageAdapter = storageAdapter;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private T _data = data ?? new();
    private bool _isLoaded = data != null;

    public async Task Read(CancellationToken token = default)
    {
        await _gate.WaitAsync(token);
        try
        {
            await ReadCore(token);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task Write(CancellationToken token = default)
    {
        await _gate.WaitAsync(token);
        try
        {
            await WriteCore(token);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<T> Get(CancellationToken token = default)
    {
        await _gate.WaitAsync(token);
        try
        {
            await EnsureDatabaseLoaded(token);
            return _data;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task Update(Action<T> updateAction, bool autoSave = true, CancellationToken token = default)
    {
        await _gate.WaitAsync(token);
        try
        {
            await EnsureDatabaseLoaded(token);
            updateAction(_data);

            if (autoSave is true)
            {
                await WriteCore(token);
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    public void Dispose()
    {
        _gate.Dispose();
        GC.SuppressFinalize(this);
    }

    private async Task ReadCore(CancellationToken token = default) =>
        _data = await _storageAdapter.Read(token) ?? new T();

    private async Task WriteCore(CancellationToken token = default) =>
        await _storageAdapter.Write(_data, token);

    private async Task EnsureDatabaseLoaded(CancellationToken token = default)
    {
        if (_isLoaded is false)
        {
            await ReadCore(token);
            _isLoaded = true;
        }
    }
}
