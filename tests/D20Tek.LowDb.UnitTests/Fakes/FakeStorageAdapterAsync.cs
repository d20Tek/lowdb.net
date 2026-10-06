using System.Text.Json;

namespace D20Tek.LowDb.UnitTests.Fakes;

/// <summary>
/// A simple in-memory asynchronous storage adapter used to test decorators without touching
/// the file system. Optionally throws a <see cref="JsonException"/> from <see cref="Read"/>
/// to simulate a corrupt primary store.
/// </summary>
internal class FakeStorageAdapterAsync<T> : IStorageAdapterAsync<T>
    where T : class
{
    private T? _value;

    public bool ThrowOnRead { get; set; }

    public int ReadCount { get; private set; }

    public int WriteCount { get; private set; }

    public Task<T?> Read(CancellationToken token = default)
    {
        ReadCount++;
        if (ThrowOnRead)
        {
            throw new JsonException("Simulated corrupt primary store.");
        }

        return Task.FromResult(_value);
    }

    public Task Write(T data, CancellationToken token = default)
    {
        WriteCount++;
        _value = data;
        return Task.CompletedTask;
    }
}
