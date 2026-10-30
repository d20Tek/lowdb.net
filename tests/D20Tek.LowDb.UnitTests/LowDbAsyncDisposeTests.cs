using D20Tek.LowDb.Adapters;
using D20Tek.LowDb.UnitTests.Entities;
using D20Tek.LowDb.UnitTests.Fakes;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.LowDb.UnitTests;

[TestClass]
public class LowDbAsyncDisposeTests
{
    [TestMethod]
    public void Dispose_WhenCalled_DoesNotThrow()
    {
        // Arrange
        var adapter = new MemoryStorageAdapterAsync<TestDocument>();
        var db = new LowDbAsync<TestDocument>(adapter);

        // Act
        var act = () => db.Dispose();

        // Assert
        act.Should().NotThrow();
    }

    [TestMethod]
    public void Dispose_CalledMultipleTimes_DoesNotThrow()
    {
        // Arrange
        var adapter = new MemoryStorageAdapterAsync<TestDocument>();
        var db = new LowDbAsync<TestDocument>(adapter);

        // Act
        db.Dispose();
        var act = () => db.Dispose();

        // Assert
        act.Should().NotThrow();
    }

    [TestMethod]
    public async Task Dispose_AfterOperations_DisposesGateWithoutThrowing()
    {
        // Arrange
        var adapter = new MemoryStorageAdapterAsync<TestDocument>();
        var db = new LowDbAsync<TestDocument>(adapter);
        await db.Update(x => x.Entities.Add(new TestEntity { Id = 1, Name = "Test" }), token: CancellationToken.None);

        // Act
        var act = () => db.Dispose();

        // Assert
        act.Should().NotThrow();
    }

    [TestMethod]
    public async Task DisposeAsync_WithAutoFlushDisabled_DoesNotPersistPendingChanges()
    {
        // Arrange
        var adapter = new FakeStorageAdapterAsync<TestDocument>();
        var db = new LowDbAsync<TestDocument>(adapter);
        await db.Update(
            x => x.Entities.Add(TestEntityFactory.Create(1)),
            autoSave: false,
            token: TestContext.CancellationToken);

        // Act
        await db.DisposeAsync();

        // Assert
        adapter.WriteCount.Should().Be(0);
    }

    [TestMethod]
    public async Task DisposeAsync_WithAutoFlushEnabledAndPendingChanges_PersistsDocument()
    {
        // Arrange
        var adapter = new FakeStorageAdapterAsync<TestDocument>();
        var db = new LowDbAsync<TestDocument>(adapter, autoFlushOnDispose: true);
        await db.Update(
            x => x.Entities.Add(TestEntityFactory.Create(1)),
            autoSave: false,
            token: TestContext.CancellationToken);

        // Act
        await db.DisposeAsync();

        // Assert
        adapter.WriteCount.Should().Be(1);
    }

    [TestMethod]
    public async Task DisposeAsync_WithAutoFlushEnabledAndNoPendingChanges_DoesNotWrite()
    {
        // Arrange
        var adapter = new FakeStorageAdapterAsync<TestDocument>();
        var db = new LowDbAsync<TestDocument>(adapter, autoFlushOnDispose: true);

        // Act
        await db.DisposeAsync();

        // Assert
        adapter.WriteCount.Should().Be(0);
    }

    [TestMethod]
    public async Task DisposeAsync_WhenAutoFlushing_RaisesChangedEvent()
    {
        // Arrange
        var adapter = new FakeStorageAdapterAsync<TestDocument>();
        var db = new LowDbAsync<TestDocument>(adapter, autoFlushOnDispose: true);
        var raised = 0;
        db.Changed += (_, _) => raised++;
        await db.Update(
            x => x.Entities.Add(TestEntityFactory.Create(1)),
            autoSave: false,
            token: TestContext.CancellationToken);

        // Act
        await db.DisposeAsync();

        // Assert
        raised.Should().Be(1);
    }

    [TestMethod]
    public async Task DisposeAsync_WhenNotFlushing_DoesNotRaiseChangedEvent()
    {
        // Arrange
        var adapter = new FakeStorageAdapterAsync<TestDocument>();
        var db = new LowDbAsync<TestDocument>(adapter, autoFlushOnDispose: true);
        var raised = 0;
        db.Changed += [ExcludeFromCodeCoverage](_, _) => raised++;

        // Act
        await db.DisposeAsync();

        // Assert
        raised.Should().Be(0);
    }

    [TestMethod]
    public async Task DisposeAsync_AfterAutoSavedUpdate_DoesNotWriteAgain()
    {
        // Arrange
        var adapter = new FakeStorageAdapterAsync<TestDocument>();
        var db = new LowDbAsync<TestDocument>(adapter, autoFlushOnDispose: true);
        await db.Update(
            x => x.Entities.Add(TestEntityFactory.Create(1)),
            token: TestContext.CancellationToken);

        // Act
        await db.DisposeAsync();

        // Assert
        adapter.WriteCount.Should().Be(1);
    }

    [TestMethod]
    public async Task DisposeAsync_CalledMultipleTimes_DoesNotThrowOrWriteTwice()
    {
        // Arrange
        var adapter = new FakeStorageAdapterAsync<TestDocument>();
        var db = new LowDbAsync<TestDocument>(adapter, autoFlushOnDispose: true);
        await db.Update(
            x => x.Entities.Add(TestEntityFactory.Create(1)),
            autoSave: false,
            token: TestContext.CancellationToken);
        await db.DisposeAsync();

        // Act
        var act = async () => await db.DisposeAsync();

        // Assert
        await act.Should().NotThrowAsync();
        adapter.WriteCount.Should().Be(1);
    }

    [TestMethod]
    public async Task DisposeAsync_AfterSynchronousDispose_DoesNotThrow()
    {
        // Arrange
        var adapter = new FakeStorageAdapterAsync<TestDocument>();
        var db = new LowDbAsync<TestDocument>(adapter, autoFlushOnDispose: true);
        await db.Update(
            x => x.Entities.Add(TestEntityFactory.Create(1)),
            autoSave: false,
            token: TestContext.CancellationToken);
        db.Dispose();

        // Act
        var act = async () => await db.DisposeAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }

    [TestMethod]
    public async Task DisposeAsync_WithAwaitUsingAndAutoFlush_PersistsDocument()
    {
        // Arrange
        var adapter = new FakeStorageAdapterAsync<TestDocument>();

        // Act
        await using (var db = new LowDbAsync<TestDocument>(adapter, autoFlushOnDispose: true))
        {
            await db.Update(
                x => x.Entities.Add(TestEntityFactory.Create(1)),
                autoSave: false,
                token: TestContext.CancellationToken);
        }

        // Assert
        adapter.WriteCount.Should().Be(1);
    }

    public TestContext TestContext { get; set; } = default!;
}
