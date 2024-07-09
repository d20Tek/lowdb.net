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
        db.Write();

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
    [ExcludeFromCodeCoverage]
    [ExpectedException(typeof(ArgumentException))]
    public void Read_WithEmptyFilename_ThrowsException()
    {
        // arrange
        var jsonAdapter = new JsonFileAdapter<TestDocument>("");
        var db = new LowDb<TestDocument>(jsonAdapter);


        // act
        db.Read();

        // assert
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    [ExpectedException(typeof(ArgumentException))]
    public void Write_WithEmptyFilename_ThrowsException()
    {
        // arrange
        var jsonAdapter = new JsonFileAdapter<TestDocument>("");
        var db = new LowDb<TestDocument>(jsonAdapter);


        // act
        db.Write();

        // assert
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
}