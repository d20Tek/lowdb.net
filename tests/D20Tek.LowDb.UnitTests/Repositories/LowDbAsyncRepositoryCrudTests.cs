using D20Tek.LowDb.Adapters;
using D20Tek.LowDb.UnitTests.Entities;

namespace D20Tek.LowDb.UnitTests.Repositories;

[TestClass]
public class LowDbAsyncRepositoryCrudTests
{
    private const string _addTestFile = "add-test-file-async.json";
    private const string _addRangeTestFile = "add-range-test-file-async.json";
    private const string _removeTestFile = "remove-test-file-async.json";
    private const string _removeRangeTestFile = "remove-range-test-file-async.json";
    private const string _updateTestFile = "update-test-file-async.json";

    [TestMethod]
    public async Task AddAsync_NewEntity_PersistsToFile()
    {
        // arrange
        var repo = LoadRepository(_addTestFile);

        var id = Guid.NewGuid().GetHashCode();

        // act
        var result = await repo.AddAsync(
            new TestEntity
                {
                    Id = id,
                    Name = "Test entity",
                    Description = "test desc.",
                    Flag = true
                });

        _ = await repo.SaveChangesAsync();

        // assert
        Assert.IsTrue(result.IsSuccess);

        // validate retrieval
        var readRepo = LoadRepository(_addTestFile);
        var getResult = await readRepo.GetByIdAsync(e => e.Id, id);
        Assert.IsTrue(getResult.IsSuccess);
        Assert.AreEqual(id, getResult.GetValue().Id);
    }

    [TestMethod]
    public async Task AddAsync_SameEntityTwice_ReturnsFailure()
    {
        // arrange
        var repo = LoadRepository(_addTestFile);
        var entity = new TestEntity
        {
            Id = Guid.NewGuid().GetHashCode(),
            Name = "Test entity",
            Description = "test desc.",
            Flag = true
        };

        // act
        var r = await repo.AddAsync(entity);
        var result = await repo.AddAsync(entity);

        // assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Entity.AddFailed", result.GetErrors().First().Code);
    }

    [TestMethod]
    public async Task AddAsyncWithoutSave_NewEntity_DoesnotPersistsToFile()
    {
        // arrange
        var repo = LoadRepository(_addTestFile);

        var id = Guid.NewGuid().GetHashCode();

        // act
        var result = await repo.AddAsync(
            new TestEntity
            {
                Id = id,
                Name = "Test entity",
                Description = "test desc.",
                Flag = true
            });

        // assert
        Assert.IsTrue(result.IsSuccess);

        // validate retrieval
        var readRepo = LoadRepository(_addTestFile);
        var getResult = await readRepo.GetByIdAsync(e => e.Id, id);
        Assert.IsTrue(getResult.IsFailure);
    }

    [TestMethod]
    public async Task AddRangeAsync_NewEntities_PersistsToFile()
    {
        // arrange
        var repo = LoadRepository(_addRangeTestFile);
        var id = Guid.NewGuid().GetHashCode();
        var tests = new List<TestEntity>()
        {
            new() { Id = id, Name = "Test entity", Description = "test desc.", Flag = true },
            new() { Id = Guid.NewGuid().GetHashCode(), Name = "Test entity2", Description = "test desc 2.", Flag = true }
        };

        // act
        var result = await repo.AddRangeAsync(tests);
        _ = await repo.SaveChangesAsync();

        // assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(2, result.GetValue().Count());

        // validate retrieval
        var readRepo = LoadRepository(_addRangeTestFile);
        var getResult = await readRepo.GetByIdAsync(e => e.Id, id);
        Assert.IsTrue(getResult.IsSuccess);
        Assert.AreEqual(id, getResult.GetValue().Id);
    }

    [TestMethod]
    public async Task AddRangeAsync_SameEntityTwice_ReturnsFailure()
    {
        // arrange
        var repo = LoadRepository(_addTestFile);
        var entity = new TestEntity
        {
            Id = Guid.NewGuid().GetHashCode(),
            Name = "Test entity",
            Description = "test desc.",
            Flag = true
        };

        // act
        var r = await repo.AddAsync(entity);
        var result = await repo.AddRangeAsync([entity]);

        // assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Entity.AddFailed", result.GetErrors().First().Code);
    }

    [TestMethod]
    public async Task RemoveAsync_ExistingEntity_PersistsToFile()
    {
        // arrange
        var repo = LoadRepository(_removeTestFile);
        var addedEntity = await AddTestEntity(repo);

        // act
        var result = await repo.RemoveAsync(addedEntity);
        _ = await repo.SaveChangesAsync();

        // assert
        Assert.IsTrue(result.IsSuccess);

        // validate retrieval
        var readRepo = LoadRepository(_removeTestFile);
        var getResult = await readRepo.GetByIdAsync(e => e.Id, addedEntity.Id);
        Assert.IsFalse(getResult.IsSuccess);
    }

    [TestMethod]
    public async Task RemoveAsync_MissingEntity_ReturnsFailure()
    {
        // arrange
        var repo = LoadRepository(_removeTestFile);
        var entity = new TestEntity
        {
            Id = Guid.NewGuid().GetHashCode(),
            Name = "Test entity",
            Description = "test desc.",
            Flag = true
        };

        // act
        var result = await repo.RemoveAsync(entity);

        // assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Entity.RemoveFailed", result.GetErrors().First().Code);
    }

    [TestMethod]
    public async Task RemoveRangeAsync_ExistingEntities_PersistsToFile()
    {
        // arrange
        var repo = LoadRepository(_removeRangeTestFile);
        var addedEntity = await AddTestEntity(repo);

        // act
        var result = await repo.RemoveRangeAsync([addedEntity]);
        _ = await repo.SaveChangesAsync();

        // assert
        Assert.IsTrue(result.IsSuccess);

        // validate retrieval
        var readRepo = LoadRepository(_removeRangeTestFile);
        var getResult = await readRepo.GetByIdAsync(e => e.Id, addedEntity.Id);
        Assert.IsFalse(getResult.IsSuccess);
    }

    [TestMethod]
    public async Task RemoveRangeAsync_MissingEntity_ReturnsFailure()
    {
        // arrange
        var repo = LoadRepository(_removeTestFile);
        var entity = new TestEntity
        {
            Id = Guid.NewGuid().GetHashCode(),
            Name = "Test entity",
            Description = "test desc.",
            Flag = true
        };

        // act
        var result = await repo.RemoveRangeAsync([entity]);

        // assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Entity.RemoveFailed", result.GetErrors().First().Code);
    }

    [TestMethod]
    public async Task UpdateAsync_ExistingEntity_PersistsToFile()
    {
        // arrange
        var repo = LoadRepository(_updateTestFile);
        var addedEntity = await AddTestEntity(repo);

        // act
        addedEntity.Name = "Updated";
        var result = await repo.UpdateAsync(addedEntity);
        _ = await repo.SaveChangesAsync();

        // assert
        Assert.IsTrue(result.IsSuccess);

        // validate retrieval
        var readRepo = LoadRepository(_updateTestFile);
        var getResult = await readRepo.GetByIdAsync(e => e.Id, addedEntity.Id);
        Assert.IsTrue(getResult.IsSuccess);
        Assert.AreEqual("Updated", getResult.GetValue().Name);
    }

    private static TestsRepositoryAsync LoadRepository(string filename)
    {
        var jsonAdapter = new JsonFileAdapterAsync<TestsRepositoryAsync.Document>(filename);
        var db = new LowDbAsync<TestsRepositoryAsync.Document>(jsonAdapter);
        return new TestsRepositoryAsync(db);
    }

    private static async Task<TestEntity> AddTestEntity(TestsRepositoryAsync repo)
    {
        var result = await repo.AddAsync(
            new TestEntity
            {
                Id = Guid.NewGuid().GetHashCode(),
                Name = "Test entity",
                Description = "test desc.",
                Flag = true
            });
        _ = await repo.SaveChangesAsync();

        return result.GetValue();
    }
}
