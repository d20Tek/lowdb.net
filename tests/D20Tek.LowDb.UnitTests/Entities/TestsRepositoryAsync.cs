using D20Tek.LowDb.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.LowDb.UnitTests.Entities;

internal interface ITestsRepositoryAsync : IRepositoryAsync<TestEntity>;

internal sealed class TestsRepositoryAsync(LowDbAsync<TestsRepositoryAsync.Document> db) :
    LowDbAsyncRepository<TestEntity, TestsRepositoryAsync.Document>(db, doc => doc.Tests), ITestsRepositoryAsync
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
