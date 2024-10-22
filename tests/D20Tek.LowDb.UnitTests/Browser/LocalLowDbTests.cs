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
public class LocalLowDbTests
{
    private readonly FakeLocalStorage _storage = new();

    [TestMethod]
    public void Read_WithMissingFile_ReturnsNull()
    {
        // arrange
        var adapter = new LocalStorageAdapter<TestDocument>("foo-keyname", _storage);
        var db = new LowDb<TestDocument>(adapter);

        // act
        db.Read();
        var result = db.Get();

        // assert
        result.Entities.Should().BeEmpty();
    }

    [TestMethod]
    public void Update_WithChanges_SavesKey()
    {
        // arrange
        var adapter = new LocalStorageAdapter<TestDocument>("update-test-entry", _storage);
        var db = new LowDb<TestDocument>(adapter);

        var id = -1;

        // act
        db.Update(x =>
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

        db.Read();
        var result = db.Get();

        // assert
        result.Should().NotBeNull();
        result!.Entities.Any(x => x.Id == id).Should().BeTrue();
    }

    [TestMethod]
    public void Update_MissingKey_CreatesEmptyDataObject()
    {
        // arrange
        var adapter = new LocalStorageAdapter<TestDocument>("empty-key", _storage);
        var db = new LowDb<TestDocument>(adapter);

        var id = -1;

        // act
        db.Update(x => id = x.LastId);

        // assert
        id.Should().Be(0);
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    [ExpectedException(typeof(ArgumentException))]
    public void Write_WithEmptyKeyname_ThrowsException()
    {
        // arrange
        var adapter = new LocalStorageAdapter<TestDocument>("", _storage);
        var db = new LowDb<TestDocument>(adapter);

        // act
        db.Write();

        // assert
    }


    [TestMethod]
    public void UpdateListType_WithChanges_SavesKey()
    {
        // arrange
        var adapter = new LocalStorageAdapter<List<TestEntity>>("update-list-test-key", _storage);
        var db = new LowDb<List<TestEntity>>(adapter);

        var id = -1;

        // act
        db.Update(x =>
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

        db.Read();
        var result = db.Get();

        // assert
        result.Should().NotBeNull();
        result.Any(x => x.Id == id).Should().BeTrue();
    }

    [TestMethod]
    public void Update_WithAutoSaveFalse_DoesNotSaveKey()
    {
        // arrange
        var adapter = new LocalStorageAdapter<List<Guid>>("nonautosave-test-key", _storage);
        var db = new LowDb<List<Guid>>(adapter);

        Guid guid = Guid.Empty;

        // act
        db.Update(x =>
        {
            guid = Guid.NewGuid();
            x.Add(guid);
        },
        false);

        db.Read();
        var result = db.Get();

        // assert
        result.Should().NotBeNull();
        result.Any().Should().BeFalse();
    }

    [TestMethod]
    public void Update_BatchMultipleChanges_SavesKey()
    {
        // arrange
        var adapter = new LocalStorageAdapter<List<Guid>>("batch-test-key", _storage);
        var db = new LowDb<List<Guid>>(adapter);

        List<Guid> expected = [];

        // act
        db.Update(x =>
        {
            var guid = Guid.NewGuid();
            expected.Add(guid);
            x.Add(guid);
        },
        false);
        db.Update(x =>
        {
            var guid = Guid.NewGuid();
            expected.Add(guid);
            x.Add(guid);
        },
        false);
        db.Update(x =>
        {
            var guid = Guid.NewGuid();
            expected.Add(guid);
            x.Add(guid);
        },
        false);

        // delayed save.
        db.Write();

        // force a reload from file.
        db.Read();
        var result = db.Get();

        // assert
        result.Should().NotBeNull();
        result.Should().Contain(expected);
    }
}