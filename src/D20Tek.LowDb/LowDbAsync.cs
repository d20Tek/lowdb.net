//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace D20Tek.LowDb;

public class LowDbAsync<T>
    where T : class, new()
{
    private readonly IFileAdapterAsync<T> _fileAdapter;
    private T? _data;

    public LowDbAsync(IFileAdapterAsync<T> fileAdapter, T? data = null)
    {
        _fileAdapter = fileAdapter;
        _data = data;
    }

    public async Task Read()
    {
        var data = await _fileAdapter.Read();
         _data = data;
    }

    public async Task Write()
    {
        if (_data is not null)
        {
            await _fileAdapter.Write(_data);
        }
    }

    public async Task Update(Action<T> updateAction)
    {
        if (_data is null)
        {
            await Read();
        }

        if (_data is null)
        {
            throw new InvalidOperationException("Attempting to perform operation on a null database member.");
        }

        updateAction(_data!);
        await Write();
    }
}
