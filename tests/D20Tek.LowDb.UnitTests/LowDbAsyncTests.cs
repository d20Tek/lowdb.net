//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.LowDb.Adapters;
using D20Tek.LowDb.UnitTests.Entities;
using FluentAssertions;

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
        await db.Read(TestContext.CancellationToken);
        var result = await db.Get(TestContext.CancellationToken);

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
        await db.Write(TestContext.CancellationToken);

        // assert
    }

    [TestMethod]
    public async Task Update_WithChanges_SavesFile()
    {
        // arrange
        var jsonAdapter = new JsonFileAdapterAsync<TestDocument>("async-update-test-file.json");
        var db = new LowDbAsync<TestDocument>(jsonAdapter);
        await db.Write(TestContext.CancellationToken);
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
        }, token: TestContext.CancellationToken);

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
        await db.Update(x => id = x.LastId, token: TestContext.CancellationToken);

        // assert
        id.Should().Be(0);
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
        }, token: TestContext.CancellationToken);

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
        false,
        TestContext.CancellationToken);

        await db.Read(TestContext.CancellationToken);
        var result = await db.Get(TestContext.CancellationToken);

        // assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Update_BatchMultipleChanges_SavesFile()
    {
        // arrange
        var jsonAdapter = new JsonFileAdapterAsync<List<Guid>>("async-batch-test-file.json");
        var db = new LowDbAsync<List<Guid>>(jsonAdapter);
        List<Guid> expected = [];

        // act
        await db.Update(x =>
        {
            var guid = Guid.NewGuid();
            expected.Add(guid);
            x.Add(guid);
        },
        false,
        TestContext.CancellationToken);

        // delayed save.
        await db.Write(TestContext.CancellationToken);

        // force a reload from file.
        await db.Read(TestContext.CancellationToken);
        var result = await db.Get(TestContext.CancellationToken);

        // assert
        result.Should().NotBeNull();
        result.Should().Contain(expected);
    }

    [TestMethod]
    public async Task Get_WithDataInitialization_ReturnsDocument()
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

        var adapter = new MemoryStorageAdapterAsync<TestDocument>();
        var db = new LowDbAsync<TestDocument>(adapter, document);

        // act
        var result = await db.Get(TestContext.CancellationToken);

        // assert
        result.Entities.Should().NotBeEmpty();
        result.Entities.Any(x => x.Id == 3).Should().BeTrue();
    }

    public TestContext TestContext { get; set; }
}