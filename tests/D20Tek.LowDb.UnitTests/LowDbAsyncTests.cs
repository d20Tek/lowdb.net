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
        result.Should().BeNull();
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
        var jsonAdapter = new MemoryAdapterAsync<TestDocument>();
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
}