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

    [TestMethod]
    public void Build_WithBackup_CreatesValidDb()
    {
        // arrange
        var builder = new LowDbBuilder()
            .UseFileDatabase("builder-with-backup.json")
            .WithBackup();

        // act
        var db = builder.Build<TestDocument>();

        // assert
        db.Should().NotBeNull();
        db.Get().Entities.Should().BeEmpty();
    }

    [TestMethod]
    public async Task BuildAsync_WithBackup_CreatesValidDb()
    {
        // arrange
        var builder = new LowDbBuilder()
            .UseFileDatabase("builder-with-backup-async.json")
            .WithBackup();

        // act
        var db = builder.BuildAsync<TestDocument>();

        // assert
        db.Should().NotBeNull();
        var result = await db.Get(TestContext.CancellationToken);
        result.Entities.Should().BeEmpty();
    }

    [TestMethod]
    public void Build_WithBackupAndInMemoryDatabase_CreatesValidDb()
    {
        // arrange
        var builder = new LowDbBuilder()
            .UseInMemoryDatabase()
            .WithBackup();

        // act
        var db = builder.Build<TestDocument>();

        // assert
        db.Should().NotBeNull();
        db.Get().Entities.Should().BeEmpty();
    }

    [TestMethod]
    public async Task BuildAsync_WithAutoFlushOnDispose_FlushesPendingChangesOnDispose()
    {
        // arrange
        var builder = new LowDbBuilder()
            .UseInMemoryDatabase()
            .WithAutoFlushOnDispose();
        var db = builder.BuildAsync<TestDocument>();
        await db.Update(
            x => x.Entities.Add(new TestEntity { Id = 1, Name = "Test" }),
            autoSave: false,
            token: TestContext.CancellationToken);

        // act
        await db.DisposeAsync();

        // assert
        db.HasPendingChanges.Should().BeFalse();
    }

    [TestMethod]
    public async Task BuildAsync_WithoutAutoFlushOnDispose_LeavesPendingChangesOnDispose()
    {
        // arrange
        var builder = new LowDbBuilder().UseInMemoryDatabase();
        var db = builder.BuildAsync<TestDocument>();
        await db.Update(
            x => x.Entities.Add(new TestEntity { Id = 1, Name = "Test" }),
            autoSave: false,
            token: TestContext.CancellationToken);

        // act
        await db.DisposeAsync();

        // assert
        db.HasPendingChanges.Should().BeTrue();
    }

    public TestContext TestContext { get; set; } = default!;
}
