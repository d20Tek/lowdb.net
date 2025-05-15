using D20Tek.LowDb.Adapters;
using D20Tek.LowDb.UnitTests.Entities;

namespace D20Tek.LowDb.UnitTests.Repositories;

[TestClass]
public class FileRepositoryCrudTests
{
    private const string _addTestFile = "add-test-file.json";
    private const string _addRangeTestFile = "add-range-test-file.json";
    private const string _removeTestFile = "remove-test-file.json";
    private const string _removeRangeTestFile = "remove-range-test-file.json";
    private const string _updateTestFile = "update-test-file.json";

    [TestMethod]
    public void Add_NewEntity_PersistsToFile()
    {
        // arrange
        var repo = LoadRepository(_addTestFile);

        var id = Guid.NewGuid().GetHashCode();

        // act
        var result = repo.Add(
            new TestEntity
                {
                    Id = id,
                    Name = "Test entity",
                    Description = "test desc.",
                    Flag = true
                });

        _ = repo.SaveChanges();

        // assert
        Assert.IsTrue(result.IsSuccess);

        // validate retrieval
        var readRepo = LoadRepository(_addTestFile);
        var getResult = readRepo.GetById(e => e.Id, id);
        Assert.IsTrue(getResult.IsSuccess);
        Assert.AreEqual(id, getResult.GetValue().Id);
    }

    [TestMethod]
    public void Add_SameEntityTwice_ReturnsFailure()
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
        var r = repo.Add(entity);
        var result = repo.Add(entity);

        // assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Entity.AddFailed", result.GetErrors().First().Code);
    }

    [TestMethod]
    public void AddWithoutSave_NewEntity_DoesnotPersistsToFile()
    {
        // arrange
        var repo = LoadRepository(_addTestFile);

        var id = Guid.NewGuid().GetHashCode();

        // act
        var result = repo.Add(
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
        var getResult = readRepo.GetById(e => e.Id, id);
        Assert.IsTrue(getResult.IsFailure);
    }

    [TestMethod]
    public void AddRange_NewEntities_PersistsToFile()
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
        var result = repo.AddRange(tests);
        _ = repo.SaveChanges();

        // assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(2, result.GetValue().Count());

        // validate retrieval
        var readRepo = LoadRepository(_addRangeTestFile);
        var getResult = readRepo.GetById(e => e.Id, id);
        Assert.IsTrue(getResult.IsSuccess);
        Assert.AreEqual(id, getResult.GetValue().Id);
    }

    [TestMethod]
    public void AddRange_SameEntityTwice_ReturnsFailure()
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
        var r = repo.Add(entity);
        var result = repo.AddRange([entity]);

        // assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Entity.AddFailed", result.GetErrors().First().Code);
    }

    [TestMethod]
    public void Remove_ExistingEntity_PersistsToFile()
    {
        // arrange
        var repo = LoadRepository(_removeTestFile);
        var addedEntity = AddTestEntity(repo);

        // act
        var result = repo.Remove(addedEntity);
        _ = repo.SaveChanges();

        // assert
        Assert.IsTrue(result.IsSuccess);

        // validate retrieval
        var readRepo = LoadRepository(_removeTestFile);
        var getResult = readRepo.GetById(e => e.Id, addedEntity.Id);
        Assert.IsFalse(getResult.IsSuccess);
    }

    [TestMethod]
    public void Remove_MissingEntity_ReturnsFailure()
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
        var result = repo.Remove(entity);

        // assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Entity.RemoveFailed", result.GetErrors().First().Code);
    }

    [TestMethod]
    public void RemoveRange_ExistingEntities_PersistsToFile()
    {
        // arrange
        var repo = LoadRepository(_removeRangeTestFile);
        var addedEntity = AddTestEntity(repo);

        // act
        var result = repo.RemoveRange([addedEntity]);
        _ = repo.SaveChanges();

        // assert
        Assert.IsTrue(result.IsSuccess);

        // validate retrieval
        var readRepo = LoadRepository(_removeRangeTestFile);
        var getResult = readRepo.GetById(e => e.Id, addedEntity.Id);
        Assert.IsFalse(getResult.IsSuccess);
    }

    [TestMethod]
    public void RemoveRange_MissingEntity_ReturnsFailure()
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
        var result = repo.RemoveRange([entity]);

        // assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Entity.RemoveFailed", result.GetErrors().First().Code);
    }

    [TestMethod]
    public void Update_ExistingEntity_PersistsToFile()
    {
        // arrange
        var repo = LoadRepository(_updateTestFile);
        var addedEntity = AddTestEntity(repo);

        // act
        addedEntity.Name = "Updated";
        var result = repo.Update(addedEntity);
        _ = repo.SaveChanges();

        // assert
        Assert.IsTrue(result.IsSuccess);

        // validate retrieval
        var readRepo = LoadRepository(_updateTestFile);
        var getResult = readRepo.GetById(e => e.Id, addedEntity.Id);
        Assert.IsTrue(getResult.IsSuccess);
        Assert.AreEqual("Updated", getResult.GetValue().Name);
    }

    private static TestsRepository LoadRepository(string filename)
    {
        var jsonAdapter = new JsonFileAdapter<TestsRepository.Document>(filename);
        var db = new LowDb<TestsRepository.Document>(jsonAdapter);
        return new TestsRepository(db);
    }

    private static TestEntity AddTestEntity(TestsRepository repo)
    {
        var result = repo.Add(
            new TestEntity
            {
                Id = Guid.NewGuid().GetHashCode(),
                Name = "Test entity",
                Description = "test desc.",
                Flag = true
            });
        _ = repo.SaveChanges();

        return result.GetValue();
    }
}
