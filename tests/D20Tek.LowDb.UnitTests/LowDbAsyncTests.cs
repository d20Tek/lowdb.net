//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.LowDb.Adapters;
using D20Tek.LowDb.UnitTests.Entities;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.LowDb.UnitTests;

[TestClass]
public class LowDbAsyncTests
{
    [TestMethod]
    public async Task Read_WithMissingFile_ReturnsNull()
    {
        // arrange
        var jsonAdapter = new JsonFileAdapterAsync<TestDocument>("async-foo-file.json");
        var db = new LowDbAsync<TestDocument>(jsonAdapter);

        // act
        await db.Read();
        var result = await db.Get();

        // assert
        result.Entities.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Write_WithFilename_SavesEmptyFile()
    {
        // arrange
        var jsonAdapter = new JsonFileAdapterAsync<TestDocument>("test-folder\\async-test-file.json");
        var db = new LowDbAsync<TestDocument>(jsonAdapter);

        // act
        await db.Write();

        // assert
    }

    [TestMethod]
    public async Task Update_WithChanges_SavesFile()
    {
        // arrange
        var jsonAdapter = new JsonFileAdapterAsync<TestDocument>("async-update-test-file.json");
        var db = new LowDbAsync<TestDocument>(jsonAdapter);
        await db.Write();

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
    public async Task Update_MissingFile_CreatesEmptyDataObject()
    {
        // arrange
        var tempFile = Path.GetTempFileName();
        var jsonAdapter = new JsonFileAdapterAsync<TestDocument>(Path.GetFileName(tempFile));
        var db = new LowDbAsync<TestDocument>(jsonAdapter);

        var id = -1;

        // act
        await db.Update(x => id = x.LastId);

        // assert
        id.Should().Be(0);
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    [ExpectedException(typeof(ArgumentException))]
    public async Task Read_WithEmptyFilename_ThrowsException()
    {
        // arrange
        var jsonAdapter = new JsonFileAdapterAsync<TestDocument>("");
        var db = new LowDbAsync<TestDocument>(jsonAdapter);


        // act
        await db.Read();

        // assert
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    [ExpectedException(typeof(ArgumentException))]
    public async Task Write_WithEmptyFilename_ThrowsException()
    {
        // arrange
        var jsonAdapter = new JsonFileAdapterAsync<TestDocument>("");
        var db = new LowDbAsync<TestDocument>(jsonAdapter);


        // act
        await db.Write();

        // assert
    }

    [TestMethod]
    public async Task Update_WithMemoryAdapter_SavesData()
    {
        // arrange
        var jsonAdapter = new MemoryStorageAdapterAsync<TestDocument>();
        var db = new LowDbAsync<TestDocument>(jsonAdapter);

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
    public async Task Update_WithAutoSaveFalse_DoesNotSaveFile()
    {
        // arrange
        var jsonAdapter = new JsonFileAdapterAsync<List<Guid>>("async-nonautosave-test-file.json");
        var db = new LowDbAsync<List<Guid>>(jsonAdapter);

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
    public async Task Update_BatchMultipleChanges_SavesFile()
    {
        // arrange
        var jsonAdapter = new JsonFileAdapterAsync<List<Guid>>("async-batch-test-file.json");
        var db = new LowDbAsync<List<Guid>>(jsonAdapter);

        List<Guid> expected = new();

        // act
        var task1 = db.Update(x =>
        {
            var guid = Guid.NewGuid();
            expected.Add(guid);
            x.Add(guid);
        },
        false);

        var task2 = db.Update(x =>
        {
            var guid = Guid.NewGuid();
            expected.Add(guid);
            x.Add(guid);
        },
        false);

        var task3 = db.Update(x =>
        {
            var guid = Guid.NewGuid();
            expected.Add(guid);
            x.Add(guid);
        },
        false);

        await Task.Delay(300);
        await Task.WhenAll(task1, task2, task3);

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