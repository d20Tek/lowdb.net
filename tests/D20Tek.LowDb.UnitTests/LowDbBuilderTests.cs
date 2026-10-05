using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using D20Tek.LowDb.UnitTests.Entities;
using FluentAssertions;

namespace D20Tek.LowDb.UnitTests;

[TestClass]
public class LowDbBuilderTests
{
    [TestMethod]
    public void WithJsonSerializerOptions_NullOptions_ThrowsArgumentNullException()
    {
        // arrange
        var builder = new LowDbBuilder();

        // act & assert
        Act().Should().Throw<ArgumentNullException>();

        [ExcludeFromCodeCoverage]
        Action Act() => () => builder.WithJsonSerializerOptions(null!);
    }

    [TestMethod]
    public void Build_WithJsonSerializerOptions_CreatesValidDb()
    {
        // arrange
        var options = new JsonSerializerOptions { WriteIndented = true };
        var builder = new LowDbBuilder()
            .UseFileDatabase("builder-serializer-options.json")
            .WithJsonSerializerOptions(options);

        // act
        var db = builder.Build<TestDocument>();

        // assert
        db.Should().NotBeNull();
        db.Get().Entities.Should().BeEmpty();
    }

    [TestMethod]
    public async Task BuildAsync_WithJsonSerializerOptions_CreatesValidDb()
    {
        // arrange
        var options = new JsonSerializerOptions { WriteIndented = true };
        var builder = new LowDbBuilder()
            .UseFileDatabase("builder-serializer-options-async.json")
            .WithJsonSerializerOptions(options);

        // act
        var db = builder.BuildAsync<TestDocument>();

        // assert
        db.Should().NotBeNull();
        var result = await db.Get(TestContext.CancellationToken);
        result.Entities.Should().BeEmpty();
    }

    [TestMethod]
    public void Build_WithJsonSerializerOptionsAndInMemoryDatabase_CreatesValidDb()
    {
        // arrange
        var options = new JsonSerializerOptions { WriteIndented = true };
        var builder = new LowDbBuilder()
            .UseInMemoryDatabase()
            .WithJsonSerializerOptions(options);

        // act
        var db = builder.Build<TestDocument>();

        // assert
        db.Should().NotBeNull();
        db.Get().Entities.Should().BeEmpty();
    }

    public TestContext TestContext { get; set; } = default!;
}
