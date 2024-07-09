//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.LowDb.UnitTests.Entities;
using FluentAssertions;

namespace D20Tek.LowDb.UnitTests;

[TestClass]
public class LowDbFactoryTests
{
    [TestMethod]
    public void CreateJsonLowDb_CreateValidDb()
    {
        // arrange

        // act
        var db = LowDbFactory.CreateJsonLowDb<TestDocument>("test.json");

        // assert
        db.Should().NotBeNull();
        db.Get().Entities.Should().HaveCount(0);
    }

    [TestMethod]
    public void CreateLowDb_CreateValidDb()
    {
        // arrange

        // act
        var db = LowDbFactory.CreateLowDb<TestDocument>(b =>
                b.WithFilename("test.json"));

        // assert
        db.Should().NotBeNull();
        db.Get().Entities.Should().HaveCount(0);
    }

    [TestMethod]
    public void CreateLowDb_CreateValidInMemoryDb()
    {
        // arrange

        // act
        var db = LowDbFactory.CreateLowDb<TestDocument>(b =>
                b.WithInMemoryDb());

        // assert
        db.Should().NotBeNull();
        db.Get().Entities.Should().HaveCount(0);
    }

    [TestMethod]
    public async Task CreateJsonLowDbAsync_CreateValidDb()
    {
        // arrange

        // act
        var db = LowDbFactory.CreateJsonLowDbAsync<TestDocument>("test.json");

        // assert
        db.Should().NotBeNull();
        var result = await db.Get();
        result.Entities.Should().HaveCount(0);
    }

    [TestMethod]
    public async Task CreateLowDbAsync_CreateValidDb()
    {
        // arrange

        // act
        var db = LowDbFactory.CreateLowDbAsync<TestDocument>(b =>
                b.WithFilename("test.json")
                 .WithFolder("test-folder"));

        // assert
        db.Should().NotBeNull();
        var result = await db.Get();
        result.Entities.Should().HaveCount(0);
    }

    [TestMethod]
    public async Task CreateLowDbAsync_CreateValidInMemoryDb()
    {
        // arrange

        // act
        var db = LowDbFactory.CreateLowDbAsync<TestDocument>(b =>
                b.WithInMemoryDb());

        // assert
        db.Should().NotBeNull();
        var result = await db.Get();
        result.Entities.Should().HaveCount(0);
    }
}
