using D20Tek.LowDb.Adapters;
using D20Tek.LowDb.UnitTests.Entities;
using FluentAssertions;

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
}
