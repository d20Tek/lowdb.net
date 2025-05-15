using D20Tek.LowDb.Repositories;

namespace D20Tek.LowDb.UnitTests.Entities;

internal interface ITestsRepository : IRepository<TestEntity>;

internal sealed class TestsRepository : FileRepository<TestEntity, TestsRepository.Document>, ITestsRepository
{
    public class Foo
    {
        public string Id { get; }

        public Foo(string id) => Id = id;
    }

    public class Document : DbDocument
    {
        public HashSet<TestEntity> Tests { get; set; } = [];

        public HashSet<Foo> Foos { get; set; } = [];
    }

    public TestsRepository(LowDb<Document> db) : base(db, doc => doc.Tests) { }
}
