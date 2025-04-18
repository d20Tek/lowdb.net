//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.LowDb.Browser.Adapters;
using D20Tek.LowDb.UnitTests.Entities;
using D20Tek.LowDb.UnitTests.Fakes;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.LowDb.UnitTests.Browser;

[TestClass]
public class LocalLowDbAsyncTests
{
    private readonly FakeLocalStorage _storage = new();

    [TestMethod]
    public async Task Read_WithMissingFile_ReturnsNull()
    {
        // arrange
        var adapter = new LocalStorageAdapterAsync<TestDocument>("foo-keyname", _storage);
        var db = new LowDbAsync<TestDocument>(adapter);

        // act
        await db.Read();
        var result = await db.Get();

        // assert
        result.Entities.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Update_WithChanges_SavesKey()
    {
        // arrange
        var adapter = new LocalStorageAdapterAsync<TestDocument>("update-test-entry", _storage);
        var db = new LowDbAsync<TestDocument>(adapter);

        var id = -1;

        // act
        await db.Update(x =>
        {
            id = x.GetNextId();
            x.Entities.Add(new TestEntity
            {
                Id = id,
                Name = "Test entity",
                Description = "test desc.",
                Flag = true
            });
        });

        await db.Read();
        var result = await db.Get();

        // assert
        result.Should().NotBeNull();
        result!.Entities.Any(x => x.Id == id).Should().BeTrue();
    }

    [TestMethod]
    public async Task Update_MissingKey_CreatesEmptyDataObject()
    {
        // arrange
        var adapter = new LocalStorageAdapterAsync<TestDocument>("empty-key", _storage);
        var db = new LowDbAsync<TestDocument>(adapter);

        var id = -1;

        // act
        await db.Update(x => id = x.LastId);

        // assert
        id.Should().Be(0);
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    [ExpectedException(typeof(ArgumentException))]
    public async Task Write_WithEmptyKeyname_ThrowsException()
    {
        // arrange
        var adapter = new LocalStorageAdapterAsync<TestDocument>("", _storage);
        var db = new LowDbAsync<TestDocument>(adapter);

        // act
        await db.Write();

        // assert
    }


    [TestMethod]
    public async Task UpdateListType_WithChanges_SavesKey()
    {
        // arrange
        var adapter = new LocalStorageAdapterAsync<List<TestEntity>>("update-list-test-key", _storage);
        var db = new LowDbAsync<List<TestEntity>>(adapter);

        var id = -1;

        // act
        await db.Update(x =>
        {
            id = Guid.NewGuid().GetHashCode();
            x.Add(new TestEntity
            {
                Id = id,
                Name = "Test entity",
                Description = "test desc.",
                Flag = true
            });
        });

        await db.Read();
        var result = await db.Get();

        // assert
        result.Should().NotBeNull();
        result.Any(x => x.Id == id).Should().BeTrue();
    }

    [TestMethod]
    public async Task Update_WithAutoSaveFalse_DoesNotSaveKey()
    {
        // arrange
        var adapter = new LocalStorageAdapterAsync<List<Guid>>("nonautosave-test-key", _storage);
        var db = new LowDbAsync<List<Guid>>(adapter);

        Guid guid = Guid.Empty;

        // act
        await db.Update(x =>
        {
            guid = Guid.NewGuid();
            x.Add(guid);
        },
        false);

        await db.Read();
        var result = await db.Get();

        // assert
        result.Should().NotBeNull();
        result.Any().Should().BeFalse();
    }

    [TestMethod]
    public async Task Update_BatchMultipleChanges_SavesKey()
    {
        // arrange
        var adapter = new LocalStorageAdapterAsync<List<Guid>>("batch-test-key", _storage);
        var db = new LowDbAsync<List<Guid>>(adapter);

        List<Guid> expected = new();

        // act
        await db.Update(x =>
        {
            var guid = Guid.NewGuid();
            expected.Add(guid);
            x.Add(guid);
        },
        false);

        // delayed save.
        await db.Write();

        // force a reload from file.
        await db.Read();
        var result = await db.Get();

        // assert
        result.Should().NotBeNull();
        result.Should().Contain(expected);
    }
}