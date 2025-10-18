using D20Tek.LowDb.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.LowDb.UnitTests.Entities;

internal interface ITestsRepository : IRepository<TestEntity>;

internal sealed class TestsRepository(LowDb<TestsRepository.Document> db) :
    LowDbRepository<TestEntity, TestsRepository.Document>(db, doc => doc.Tests), ITestsRepository
{
    [ExcludeFromCodeCoverage]
    public class Foo(string id)
    {
        public string Id { get; } = id;
    }

    public class Document : DbDocument
    {
        public HashSet<TestEntity> Tests { get; set; } = [];

        public HashSet<Foo> Foos { get; set; } = [];
    }
}
