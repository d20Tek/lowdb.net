using D20Tek.LowDb.Adapters;
using D20Tek.LowDb.UnitTests.Entities;

namespace D20Tek.LowDb.UnitTests.Repositories;

[TestClass]
public class LowDbAsyncRepositoryReadTests
{
    [TestMethod]
    public async Task GetAllAsync_ReturnsEntityList()
    {
        // arrange
        var repo = InitializeTestsRepository();

        // act
        var result = await repo.GetAllAsync(TestContext.CancellationToken);

        // assert
        Assert.IsTrue(result.IsSuccess);
        var entities = result.GetValue();
        Assert.AreEqual(3, entities.Count());
        Assert.IsTrue(entities.Any(x => x.Id == 1));
    }

    [TestMethod]
    public async Task GetByIdAsync_WithExistingEntity_ReturnsEntity()
    {
        // arrange
        var repo = InitializeTestsRepository();

        // act
        var result = await repo.GetByIdAsync(e => e.Id, 2, TestContext.CancellationToken);

        // assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(2, result.GetValue().Id);
    }

    [TestMethod]
    public async Task GetByIdAsync_WithMissingId_ReturnsFailure()
    {
        // arrange
        var repo = InitializeTestsRepository();

        // act
        var result = await repo.GetByIdAsync(e => e.Id, 99, TestContext.CancellationToken);

        // assert
        Assert.IsTrue(result.IsFailure);
        Assert.Contains("NotFound", result.GetErrors().First().Code);
    }

    [TestMethod]
    public async Task FindAsync_WithExistingPredicate_ReturnsEntities()
    {
        // arrange
        var repo = InitializeTestsRepository();

        // act
        var result = await repo.FindAsync(e => e.Name == "Test 3", TestContext.CancellationToken);

        // assert
        Assert.IsTrue(result.IsSuccess);
        var entities = result.GetValue();
        Assert.AreEqual(1, entities.Count());
        Assert.IsTrue(entities.Any(x => x.Id == 3));
    }

    [TestMethod]
    public async Task FindAsync_WithMissingPredicate_ReturnsEmptyList()
    {
        // arrange
        var repo = InitializeTestsRepository();

        // act
        var result = await repo.FindAsync(e => e.Name == "missing", TestContext.CancellationToken);

        // assert
        Assert.IsTrue(result.IsSuccess);
        var entities = result.GetValue();
        Assert.AreEqual(0, entities.Count());
    }

    [TestMethod]
    public async Task ExistsAsync_WithExistingPredicate_ReturnsTrue()
    {
        // arrange
        var repo = InitializeTestsRepository();

        // act
        var result = await repo.ExistsAsync(e => e.Name == "Test 3", TestContext.CancellationToken);

        // assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(result.GetValue());
    }

    [TestMethod]
    public async Task ExistsAsync_WithMissingPredicate_ReturnsFalse()
    {
        // arrange
        var repo = InitializeTestsRepository();

        // act
        var result = await repo.ExistsAsync(e => e.Id == 99, TestContext.CancellationToken);

        // assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.GetValue());
    }

    [TestMethod]
    public async Task Exists_WithException_ReturnsFailure()
    {
        // arrange
        var repo = InitializeTestsRepository();

        // act
        var result = await repo.ExistsAsync(e => (e.Id / 0) > 1 , TestContext.CancellationToken);

        // assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("General.Exception", result.GetErrors().First().Code);
    }

    private static TestsRepositoryAsync InitializeTestsRepository()
    {
        var document = new TestsRepositoryAsync.Document
        {
            Tests = [
                new TestEntity { Id = 1, Name = "Test 1", Description = "desc 1" },
                new TestEntity { Id = 2, Name = "Test 2", Description = "desc 2" },
                new TestEntity { Id = 3, Name = "Test 3", Description = "desc 3" },
            ]
        };
        var adapter = new MemoryStorageAdapterAsync<TestsRepositoryAsync.Document>();
        var db = new LowDbAsync<TestsRepositoryAsync.Document>(adapter, document);
        return new TestsRepositoryAsync(db);
    }

    public TestContext TestContext { get; set; }
}
