using D20Tek.LowDb.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.LowDb.UnitTests.Entities;

internal interface ITestsRepositoryAsync : IRepositoryAsync<TestEntity>;

internal sealed class TestsRepositoryAsync :
    LowDbAsyncRepository<TestEntity, TestsRepositoryAsync.Document>, ITestsRepositoryAsync
{
    [ExcludeFromCodeCoverage]
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

    public TestsRepositoryAsync(LowDbAsync<Document> db) : base(db, doc => doc.Tests) { }
}
