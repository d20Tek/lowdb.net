using D20Tek.LowDb.UnitTests.Entities;
using D20Tek.LowDb.UnitTests.Fakes;
using FluentAssertions;

namespace D20Tek.LowDb.UnitTests;

[TestClass]
public class LowDbAsyncFlushTests
{
    [TestMethod]
    public async Task FlushAsync_WithPendingChanges_PersistsDocument()
    {
        // Arrange
        var adapter = new FakeStorageAdapterAsync<TestDocument>();
        using var db = new LowDbAsync<TestDocument>(adapter);
        await db.Update(
            x => x.Entities.Add(TestEntityFactory.Create(1)),
            autoSave: false,
            token: TestContext.CancellationToken);

        // Act
        await db.FlushAsync(TestContext.CancellationToken);

        // Assert
        adapter.WriteCount.Should().Be(1);
    }

    [TestMethod]
    public async Task FlushAsync_WithNoPendingChanges_StillWrites()
    {
        // Arrange
        var adapter = new FakeStorageAdapterAsync<TestDocument>();
        using var db = new LowDbAsync<TestDocument>(adapter);

        // Act
        await db.FlushAsync(TestContext.CancellationToken);

        // Assert
        adapter.WriteCount.Should().Be(1);
    }

    [TestMethod]
    public async Task FlushAsync_WithSubscriber_RaisesChangedEvent()
    {
        // Arrange
        var adapter = new FakeStorageAdapterAsync<TestDocument>();
        using var db = new LowDbAsync<TestDocument>(adapter);
        var raised = 0;
        db.Changed += (_, _) => raised++;

        // Act
        await db.FlushAsync(TestContext.CancellationToken);

        // Assert
        raised.Should().Be(1);
    }

    [TestMethod]
    public async Task FlushAsync_AfterFlush_ClearsPendingChanges()
    {
        // Arrange
        var adapter = new FakeStorageAdapterAsync<TestDocument>();
        using var db = new LowDbAsync<TestDocument>(adapter);
        await db.Update(
            x => x.Entities.Add(TestEntityFactory.Create(1)),
            autoSave: false,
            token: TestContext.CancellationToken);

        // Act
        await db.FlushAsync(TestContext.CancellationToken);

        // Assert
        db.HasPendingChanges.Should().BeFalse();
    }

    [TestMethod]
    public async Task HasPendingChanges_WithNewDatabase_ReturnsFalse()
    {
        // Arrange
        var adapter = new FakeStorageAdapterAsync<TestDocument>();
        using var db = new LowDbAsync<TestDocument>(adapter);

        // Act
        var result = await Task.FromResult(db.HasPendingChanges);

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public async Task HasPendingChanges_AfterUpdateWithAutoSaveFalse_ReturnsTrue()
    {
        // Arrange
        var adapter = new FakeStorageAdapterAsync<TestDocument>();
        using var db = new LowDbAsync<TestDocument>(adapter);

        // Act
        await db.Update(
            x => x.Entities.Add(TestEntityFactory.Create(1)),
            autoSave: false,
            token: TestContext.CancellationToken);

        // Assert
        db.HasPendingChanges.Should().BeTrue();
    }

    [TestMethod]
    public async Task HasPendingChanges_AfterUpdateWithAutoSaveTrue_ReturnsFalse()
    {
        // Arrange
        var adapter = new FakeStorageAdapterAsync<TestDocument>();
        using var db = new LowDbAsync<TestDocument>(adapter);

        // Act
        await db.Update(
            x => x.Entities.Add(TestEntityFactory.Create(1)),
            token: TestContext.CancellationToken);

        // Assert
        db.HasPendingChanges.Should().BeFalse();
    }

    [TestMethod]
    public async Task HasPendingChanges_AfterRead_ReturnsFalse()
    {
        // Arrange
        var adapter = new FakeStorageAdapterAsync<TestDocument>();
        using var db = new LowDbAsync<TestDocument>(adapter);
        await db.Update(
            x => x.Entities.Add(TestEntityFactory.Create(1)),
            autoSave: false,
            token: TestContext.CancellationToken);

        // Act
        await db.Read(TestContext.CancellationToken);

        // Assert
        db.HasPendingChanges.Should().BeFalse();
    }

    public TestContext TestContext { get; set; } = default!;
}
