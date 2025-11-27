using D20Tek.LowDb.UnitTests.Entities;

namespace D20Tek.LowDb.UnitTests.Fakes;

internal static class TestEntityFactory
{
    public static TestEntity Create(int? id = null) =>
        new()
        {
            Id = id ?? Guid.NewGuid().GetHashCode(),
            Name = "Test entity",
            Description = "test desc.",
            Flag = true
        };
}
