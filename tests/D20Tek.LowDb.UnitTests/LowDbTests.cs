//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.LowDb.Adapters;
using D20Tek.LowDb.UnitTests.Entities;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.LowDb.UnitTests;

[TestClass]
public class LowDbTests
{
    [TestMethod]
    public void Read_WithMissingFile_ReturnsNull()
    {
        // arrange
        var jsonAdapter = new JsonFileAdapter<TestDocument>("foo-file.json");
        var db = new LowDb<TestDocument>(jsonAdapter);

        // act
        db.Read();
        var result = db.Get();

        // assert
        result.Entities.Should().BeEmpty();
    }

    [TestMethod]
    public void Write_WithFilename_SavesEmptyFile()
    {
        // arrange
        var jsonAdapter = new JsonFileAdapter<TestDocument>("test-folder\\test-file.json");
        var db = new LowDb<TestDocument>(jsonAdapter);

        // act
        db.Write();

        // assert
    }

    [TestMethod]
    public void Update_WithChanges_SavesFile()
    {
        // arrange
        var jsonAdapter = new JsonFileAdapter<TestDocument>("update-test-file.json");
        var db = new LowDb<TestDocument>(jsonAdapter);

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
    public void Update_MissingFile_CreatesEmptyDataObject()
    {
        // arrange
        var tempFile = Path.GetTempFileName();
        var jsonAdapter = new JsonFileAdapter<TestDocument>(Path.GetFileName(tempFile));
        var db = new LowDb<TestDocument>(jsonAdapter);

        var id = -1;

        // act
        db.Update(x => id = x.LastId);

        // assert
        id.Should().Be(0);
    }

    [TestMethod]
    public void Update_WithMemoryAdapter_SavesData()
    {
        // arrange
        var jsonAdapter = new MemoryStorageAdapter<TestDocument>();
        var db = new LowDb<TestDocument>(jsonAdapter);

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
    public void UpdateListType_WithChanges_SavesFile()
    {
        // arrange
        var jsonAdapter = new JsonFileAdapter<List<TestEntity>>("update-list-test-file.json");
        var db = new LowDb<List<TestEntity>>(jsonAdapter);

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
    public void Update_WithAutoSaveFalse_DoesNotSaveFile()
    {
        // arrange
        var jsonAdapter = new JsonFileAdapter<List<Guid>>("nonautosave-test-file.json");
        var db = new LowDb<List<Guid>>(jsonAdapter);

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
    public void Update_BatchMultipleChanges_SavesFile()
    {
        // arrange
        var jsonAdapter = new JsonFileAdapter<List<Guid>>("batch-test-file.json");
        var db = new LowDb<List<Guid>>(jsonAdapter);

        List<Guid> expected = new();

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

    [TestMethod]
    public void Get_WithDataInitialization_ReturnsDocument()
    {
        // arrange
        var document = new TestDocument
        {
            LastId = 3,
            Version = "1.0",
            Entities =
            [
                new TestEntity { Id = 1, Name = "Test 1", Description = "desc 1" },
                new TestEntity { Id = 2, Name = "Test 2", Description = "desc 2" },
                new TestEntity { Id = 3, Name = "Test 3", Description = "desc 3" },
            ]
        };
        var adapter = new MemoryStorageAdapter<TestDocument>();
        var db = new LowDb<TestDocument>(adapter, document);

        // act
        var result = db.Get();

        // assert
        result.Entities.Should().NotBeEmpty();
        result.Entities.Any(x => x.Id == 1).Should().BeTrue();
    }
}