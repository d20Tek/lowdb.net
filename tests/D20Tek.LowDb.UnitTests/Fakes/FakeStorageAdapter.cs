using System.Text.Json;

namespace D20Tek.LowDb.UnitTests.Fakes;

/// <summary>
/// A simple in-memory synchronous storage adapter used to test decorators without touching
/// the file system. Optionally throws a <see cref="JsonException"/> from <see cref="Read"/>
/// to simulate a corrupt primary store.
/// </summary>
internal class FakeStorageAdapter<T> : IStorageAdapter<T>
    where T : class
{
    private T? _value;

    public bool ThrowOnRead { get; set; }

    public int ReadCount { get; private set; }

    public int WriteCount { get; private set; }

    public T? Read()
    {
        ReadCount++;
        if (ThrowOnRead)
        {
            throw new JsonException("Simulated corrupt primary store.");
        }

        return _value;
    }

    public void Write(T data)
    {
        WriteCount++;
        _value = data;
    }
}
