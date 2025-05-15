using D20Tek.LowDb.Adapters;
using D20Tek.LowDb.UnitTests.Entities;

namespace D20Tek.LowDb.UnitTests.Repositories;

[TestClass]
public class FileRepositoryReadTests
{
    [TestMethod]
    public void GetAll_ReturnsEntityList()
    {
        // arrange
        var repo = InitializeTestsRepository();

        // act
        var result = repo.GetAll();

        // assert
        Assert.IsTrue(result.IsSuccess);
        var entities = result.GetValue();
        Assert.AreEqual(3, entities.Count());
        Assert.IsTrue(entities.Any(x => x.Id == 1));
    }

    [TestMethod]
    public void GetById_WithExistingEntity_ReturnsEntity()
    {
        // arrange
        var repo = InitializeTestsRepository();

        // act
        var result = repo.GetById(e => e.Id, 2);

        // assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(2, result.GetValue().Id);
    }

    [TestMethod]
    public void GetById_WithMissingId_ReturnsFailure()
    {
        // arrange
        var repo = InitializeTestsRepository();

        // act
        var result = repo.GetById(e => e.Id, 99);

        // assert
        Assert.IsTrue(result.IsFailure);
        Assert.Contains("NotFound", result.GetErrors().First().Code);
    }

    [TestMethod]
    public void Find_WithExistingPredicate_ReturnsEntities()
    {
        // arrange
        var repo = InitializeTestsRepository();

        // act
        var result = repo.Find(e => e.Name == "Test 3");

        // assert
        Assert.IsTrue(result.IsSuccess);
        var entities = result.GetValue();
        Assert.AreEqual(1, entities.Count());
        Assert.IsTrue(entities.Any(x => x.Id == 3));
    }

    [TestMethod]
    public void Find_WithMissingPredicate_ReturnsEmptyList()
    {
        // arrange
        var repo = InitializeTestsRepository();

        // act
        var result = repo.Find(e => e.Name == "missing");

        // assert
        Assert.IsTrue(result.IsSuccess);
        var entities = result.GetValue();
        Assert.AreEqual(0, entities.Count());
    }

    [TestMethod]
    public void Exists_WithExistingPredicate_ReturnsTrue()
    {
        // arrange
        var repo = InitializeTestsRepository();

        // act
        var result = repo.Exists(e => e.Name == "Test 3");

        // assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(result.GetValue());
    }

    [TestMethod]
    public void Exists_WithMissingPredicate_ReturnsFalse()
    {
        // arrange
        var repo = InitializeTestsRepository();

        // act
        var result = repo.Exists(e => e.Id == 99);

        // assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.GetValue());
    }

    [TestMethod]
    public void Exists_WithException_ReturnsFailure()
    {
        // arrange
        var repo = InitializeTestsRepository();

        // act
        var result = repo.Exists(e => (e.Id / 0) > 1 );

        // assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("General.Exception", result.GetErrors().First().Code);
    }

    private static TestsRepository InitializeTestsRepository()
    {
        var document = new TestsRepository.Document
        {
            Tests = [
                new TestEntity { Id = 1, Name = "Test 1", Description = "desc 1" },
                new TestEntity { Id = 2, Name = "Test 2", Description = "desc 2" },
                new TestEntity { Id = 3, Name = "Test 3", Description = "desc 3" },
            ]
        };
        var adapter = new MemoryStorageAdapter<TestsRepository.Document>();
        var db = new LowDb<TestsRepository.Document>(adapter, document);
        return new TestsRepository(db);
    }
}
