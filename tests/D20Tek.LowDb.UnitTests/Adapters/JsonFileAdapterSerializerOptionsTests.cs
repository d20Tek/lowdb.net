using System.Text.Json;
using D20Tek.LowDb.Adapters;
using D20Tek.LowDb.UnitTests.Entities;
using D20Tek.LowDb.UnitTests.Fakes;
using FluentAssertions;

namespace D20Tek.LowDb.UnitTests.Adapters;

[TestClass]
public class JsonFileAdapterSerializerOptionsTests
{
    [TestMethod]
    public void Constructor_WithNullSerializerOptions_UsesDefaultOptions()
    {
        // arrange
        var filename = "default-options-test.json";
        var adapter = new JsonFileAdapter<TestDocument>(filename);
        var db = new LowDb<TestDocument>(adapter);

        // act
        db.Update(x => x.Entities.Add(TestEntityFactory.Create(1)));
        var rawJson = File.ReadAllText(filename);

        // assert
        rawJson.Should().Contain("\"tests\"");
    }

    [TestMethod]
    public void Constructor_WithCustomSerializerOptions_UsesSuppliedOptions()
    {
        // arrange
        var filename = "custom-options-test.json";
        var options = new JsonSerializerOptions { WriteIndented = true };
        var adapter = new JsonFileAdapter<TestDocument>(filename, options);
        var db = new LowDb<TestDocument>(adapter);

        // act
        db.Update(x => x.Entities.Add(TestEntityFactory.Create(1)));
        var rawJson = File.ReadAllText(filename);

        // assert
        rawJson.Should().Contain("\n");
    }

    [TestMethod]
    public void ReadWrite_WithCustomSerializerOptions_RoundTripsDocument()
    {
        // arrange
        var filename = "custom-options-roundtrip-test.json";
        File.Delete(filename);
        var options = new JsonSerializerOptions { WriteIndented = true };
        var adapter = new JsonFileAdapter<TestDocument>(filename, options);
        var db = new LowDb<TestDocument>(adapter);

        // act
        db.Update(x => x.Entities.Add(TestEntityFactory.Create(1)));
        db.Read();
        var result = db.Get();

        // assert
        result.Entities.Should().ContainSingle(x => x.Id == 1);
    }

    [TestMethod]
    public async Task ConstructorAsync_WithNullSerializerOptions_UsesDefaultOptions()
    {
        // arrange
        var filename = "default-options-test-async.json";
        var adapter = new JsonFileAdapterAsync<TestDocument>(filename);
        var db = new LowDbAsync<TestDocument>(adapter);

        // act
        await db.Update(x => x.Entities.Add(TestEntityFactory.Create(1)), token: TestContext.CancellationToken);
        var rawJson = await File.ReadAllTextAsync(filename, TestContext.CancellationToken);

        // assert
        rawJson.Should().Contain("\"tests\"");
    }

    [TestMethod]
    public async Task ConstructorAsync_WithCustomSerializerOptions_RoundTripsDocument()
    {
        // arrange
        var filename = "custom-options-roundtrip-test-async.json";
        File.Delete(filename);
        var options = new JsonSerializerOptions { WriteIndented = true };
        var adapter = new JsonFileAdapterAsync<TestDocument>(filename, options);
        var db = new LowDbAsync<TestDocument>(adapter);

        // act
        await db.Update(x => x.Entities.Add(TestEntityFactory.Create(1)), token: TestContext.CancellationToken);
        await db.Read(TestContext.CancellationToken);
        var result = await db.Get(TestContext.CancellationToken);

        // assert
        result.Entities.Should().ContainSingle(x => x.Id == 1);
    }

    public TestContext TestContext { get; set; } = default!;
}
