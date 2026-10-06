using D20Tek.LowDb.Adapters;
using D20Tek.LowDb.UnitTests.Entities;
using D20Tek.LowDb.UnitTests.Fakes;
using FluentAssertions;

namespace D20Tek.LowDb.UnitTests.Adapters;

[TestClass]
public class BackupStorageAdapterAsyncTests
{
    [TestMethod]
    public async Task Read_PrimaryHasValue_ReturnsPrimaryValue()
    {
        // arrange
        var primary = new FakeStorageAdapterAsync<TestDocument>();
        var backup = new FakeStorageAdapterAsync<TestDocument>();
        var document = new TestDocument();
        await primary.Write(document, TestContext.CancellationToken);
        var adapter = new BackupStorageAdapterAsync<TestDocument>(primary, backup);

        // act
        var result = await adapter.Read(TestContext.CancellationToken);

        // assert
        result.Should().BeSameAs(document);
        backup.ReadCount.Should().Be(0);
    }

    [TestMethod]
    public async Task Read_PrimaryReturnsNull_FallsBackToBackupValue()
    {
        // arrange
        var primary = new FakeStorageAdapterAsync<TestDocument>();
        var backup = new FakeStorageAdapterAsync<TestDocument>();
        var document = new TestDocument();
        await backup.Write(document, TestContext.CancellationToken);
        var adapter = new BackupStorageAdapterAsync<TestDocument>(primary, backup);

        // act
        var result = await adapter.Read(TestContext.CancellationToken);

        // assert
        result.Should().BeSameAs(document);
    }

    [TestMethod]
    public async Task Read_PrimaryAndBackupReturnNull_ReturnsNull()
    {
        // arrange
        var primary = new FakeStorageAdapterAsync<TestDocument>();
        var backup = new FakeStorageAdapterAsync<TestDocument>();
        var adapter = new BackupStorageAdapterAsync<TestDocument>(primary, backup);

        // act
        var result = await adapter.Read(TestContext.CancellationToken);

        // assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task Read_PrimaryThrowsJsonException_FallsBackToBackupValue()
    {
        // arrange
        var primary = new FakeStorageAdapterAsync<TestDocument> { ThrowOnRead = true };
        var backup = new FakeStorageAdapterAsync<TestDocument>();
        var document = new TestDocument();
        await backup.Write(document, TestContext.CancellationToken);
        var adapter = new BackupStorageAdapterAsync<TestDocument>(primary, backup);

        // act
        var result = await adapter.Read(TestContext.CancellationToken);

        // assert
        result.Should().BeSameAs(document);
    }

    [TestMethod]
    public async Task Read_PrimaryThrowsJsonExceptionAndBackupIsNull_RethrowsJsonException()
    {
        // arrange
        var primary = new FakeStorageAdapterAsync<TestDocument> { ThrowOnRead = true };
        var backup = new FakeStorageAdapterAsync<TestDocument>();
        var adapter = new BackupStorageAdapterAsync<TestDocument>(primary, backup);

        // act & assert
        await Assert.ThrowsExactlyAsync<System.Text.Json.JsonException>(Act);

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        Task Act() => adapter.Read(TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task Write_PrimaryHasExistingValue_CopiesPreviousValueToBackupThenWritesPrimary()
    {
        // arrange
        var primary = new FakeStorageAdapterAsync<TestDocument>();
        var backup = new FakeStorageAdapterAsync<TestDocument>();
        var previous = new TestDocument();
        await primary.Write(previous, TestContext.CancellationToken);
        var adapter = new BackupStorageAdapterAsync<TestDocument>(primary, backup);
        var next = new TestDocument();

        // act
        await adapter.Write(next, TestContext.CancellationToken);

        // assert
        (await backup.Read(TestContext.CancellationToken)).Should().BeSameAs(previous);
        (await primary.Read(TestContext.CancellationToken)).Should().BeSameAs(next);
    }

    [TestMethod]
    public async Task Write_PrimaryHasNoExistingValue_DoesNotWriteToBackup()
    {
        // arrange
        var primary = new FakeStorageAdapterAsync<TestDocument>();
        var backup = new FakeStorageAdapterAsync<TestDocument>();
        var adapter = new BackupStorageAdapterAsync<TestDocument>(primary, backup);
        var next = new TestDocument();

        // act
        await adapter.Write(next, TestContext.CancellationToken);

        // assert
        backup.WriteCount.Should().Be(0);
        (await primary.Read(TestContext.CancellationToken)).Should().BeSameAs(next);
    }

    [TestMethod]
    public async Task Write_PrimaryThrowsJsonExceptionOnRead_SkipsBackupCopyAndWritesPrimary()
    {
        // arrange
        var primary = new FakeStorageAdapterAsync<TestDocument> { ThrowOnRead = true };
        var backup = new FakeStorageAdapterAsync<TestDocument>();
        var adapter = new BackupStorageAdapterAsync<TestDocument>(primary, backup);
        var next = new TestDocument();

        // act
        await adapter.Write(next, TestContext.CancellationToken);

        // assert
        backup.WriteCount.Should().Be(0);
        primary.WriteCount.Should().Be(1);
    }

    public TestContext TestContext { get; set; } = default!;
}
