using D20Tek.LowDb.Adapters;
using D20Tek.LowDb.UnitTests.Entities;
using D20Tek.LowDb.UnitTests.Fakes;
using FluentAssertions;

namespace D20Tek.LowDb.UnitTests.Adapters;

[TestClass]
public class BackupStorageAdapterTests
{
    [TestMethod]
    public void Read_PrimaryHasValue_ReturnsPrimaryValue()
    {
        // arrange
        var primary = new FakeStorageAdapter<TestDocument>();
        var backup = new FakeStorageAdapter<TestDocument>();
        var document = new TestDocument();
        primary.Write(document);
        var adapter = new BackupStorageAdapter<TestDocument>(primary, backup);

        // act
        var result = adapter.Read();

        // assert
        result.Should().BeSameAs(document);
        backup.ReadCount.Should().Be(0);
    }

    [TestMethod]
    public void Read_PrimaryReturnsNull_FallsBackToBackupValue()
    {
        // arrange
        var primary = new FakeStorageAdapter<TestDocument>();
        var backup = new FakeStorageAdapter<TestDocument>();
        var document = new TestDocument();
        backup.Write(document);
        var adapter = new BackupStorageAdapter<TestDocument>(primary, backup);

        // act
        var result = adapter.Read();

        // assert
        result.Should().BeSameAs(document);
    }

    [TestMethod]
    public void Read_PrimaryAndBackupReturnNull_ReturnsNull()
    {
        // arrange
        var primary = new FakeStorageAdapter<TestDocument>();
        var backup = new FakeStorageAdapter<TestDocument>();
        var adapter = new BackupStorageAdapter<TestDocument>(primary, backup);

        // act
        var result = adapter.Read();

        // assert
        result.Should().BeNull();
    }

    [TestMethod]
    public void Read_PrimaryThrowsJsonException_FallsBackToBackupValue()
    {
        // arrange
        var primary = new FakeStorageAdapter<TestDocument> { ThrowOnRead = true };
        var backup = new FakeStorageAdapter<TestDocument>();
        var document = new TestDocument();
        backup.Write(document);
        var adapter = new BackupStorageAdapter<TestDocument>(primary, backup);

        // act
        var result = adapter.Read();

        // assert
        result.Should().BeSameAs(document);
    }

    [TestMethod]
    public void Read_PrimaryThrowsJsonExceptionAndBackupIsNull_RethrowsJsonException()
    {
        // arrange
        var primary = new FakeStorageAdapter<TestDocument> { ThrowOnRead = true };
        var backup = new FakeStorageAdapter<TestDocument>();
        var adapter = new BackupStorageAdapter<TestDocument>(primary, backup);

        // act & assert
        Act().Should().Throw<System.Text.Json.JsonException>();

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        Action Act() => () => adapter.Read();
    }

    [TestMethod]
    public void Write_PrimaryHasExistingValue_CopiesPreviousValueToBackupThenWritesPrimary()
    {
        // arrange
        var primary = new FakeStorageAdapter<TestDocument>();
        var backup = new FakeStorageAdapter<TestDocument>();
        var previous = new TestDocument();
        primary.Write(previous);
        var adapter = new BackupStorageAdapter<TestDocument>(primary, backup);
        var next = new TestDocument();

        // act
        adapter.Write(next);

        // assert
        backup.Read().Should().BeSameAs(previous);
        primary.Read().Should().BeSameAs(next);
    }

    [TestMethod]
    public void Write_PrimaryHasNoExistingValue_DoesNotWriteToBackup()
    {
        // arrange
        var primary = new FakeStorageAdapter<TestDocument>();
        var backup = new FakeStorageAdapter<TestDocument>();
        var adapter = new BackupStorageAdapter<TestDocument>(primary, backup);
        var next = new TestDocument();

        // act
        adapter.Write(next);

        // assert
        backup.WriteCount.Should().Be(0);
        primary.Read().Should().BeSameAs(next);
    }

    [TestMethod]
    public void Write_PrimaryThrowsJsonExceptionOnRead_SkipsBackupCopyAndWritesPrimary()
    {
        // arrange
        var primary = new FakeStorageAdapter<TestDocument> { ThrowOnRead = true };
        var backup = new FakeStorageAdapter<TestDocument>();
        var adapter = new BackupStorageAdapter<TestDocument>(primary, backup);
        var next = new TestDocument();

        // act
        adapter.Write(next);

        // assert
        backup.WriteCount.Should().Be(0);
        primary.WriteCount.Should().Be(1);
    }
}
