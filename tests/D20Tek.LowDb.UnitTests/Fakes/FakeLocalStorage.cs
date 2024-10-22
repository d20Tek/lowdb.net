//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using Blazored.LocalStorage;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.LowDb.UnitTests.Fakes;

[ExcludeFromCodeCoverage]
internal class FakeLocalStorage : ISyncLocalStorageService, ILocalStorageService
{
    private readonly Dictionary<string, object> _storage = [];

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable CS0067
    public event EventHandler<ChangingEventArgs> Changing;
    public event EventHandler<ChangedEventArgs> Changed;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning restore CS0067

    public void Clear() => _storage.Clear();

    public ValueTask ClearAsync(CancellationToken cancellationToken = default)
    {
        _storage.Clear();
        return ValueTask.CompletedTask;
    }

    public bool ContainKey(string key) => _storage.ContainsKey(key);

    public ValueTask<bool> ContainKeyAsync(string key, CancellationToken cancellationToken = default) =>
        ValueTask.FromResult(_storage.ContainsKey(key));

    public T? GetItem<T>(string key)
    {
        var result = _storage.TryGetValue(key, out var value);
        return result is false ? default : (T?)value;
    }

    public string? GetItemAsString(string key)
    {
        var result = _storage.TryGetValue(key, out var value);
        return result is false ? default : (string?)value;
    }

    public ValueTask<T?> GetItemAsync<T>(string key, CancellationToken cancellationToken = default) =>
        ValueTask.FromResult(GetItem<T>(key));

    public ValueTask<string?> GetItemAsStringAsync(string key, CancellationToken cancellationToken = default) =>
        ValueTask.FromResult(GetItemAsString(key));

    public string? Key(int index) => (index < 0 || index >= _storage.Count) ? null : _storage.Keys.ElementAt(index);

    public ValueTask<string?> KeyAsync(int index, CancellationToken cancellationToken = default) =>
        ValueTask.FromResult(Key(index));

    public IEnumerable<string> Keys() => _storage.Keys;

    public ValueTask<IEnumerable<string>> KeysAsync(CancellationToken cancellationToken = default) =>
        ValueTask.FromResult(Keys());

    public int Length() => _storage.Count;

    public ValueTask<int> LengthAsync(CancellationToken cancellationToken = default) => ValueTask.FromResult(_storage.Count);

    public void RemoveItem(string key) => _storage.Remove(key);

    public ValueTask RemoveItemAsync(string key, CancellationToken cancellationToken = default)
    {
        _storage.Remove(key);
        return ValueTask.CompletedTask;
    }

    public void RemoveItems(IEnumerable<string> keys) => keys.ToList().ForEach(key => _storage.Remove(key));

    public ValueTask RemoveItemsAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        RemoveItems(keys);
        return ValueTask.CompletedTask;
    }

    public void SetItem<T>(string key, T data) => _storage[key] = data!;

    public void SetItemAsString(string key, string data) => _storage[key] = data;

    public ValueTask SetItemAsync<T>(string key, T data, CancellationToken cancellationToken = default)
    {
        _storage[key] = data!;
        return ValueTask.CompletedTask;
    }

    public ValueTask SetItemAsStringAsync(string key, string data, CancellationToken cancellationToken = default)
    {
        _storage[key] = data;
        return ValueTask.CompletedTask;
    }
}
