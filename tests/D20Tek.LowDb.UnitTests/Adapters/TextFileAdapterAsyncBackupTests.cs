using D20Tek.LowDb.Adapters;
using FluentAssertions;

namespace D20Tek.LowDb.UnitTests.Adapters;

[TestClass]
public class TextFileAdapterAsyncBackupTests
{
    [TestMethod]
    public async Task Write_WithBackupDisabled_DoesNotCreateBackupFile()
    {
        // arrange
        var filename = "text-backup-disabled-async.txt";
        var backupFilename = filename + ".bak";
        File.Delete(filename);
        File.Delete(backupFilename);
        var adapter = new TextFileAdapterAsync(filename);

        // act
        await adapter.Write("first", TestContext.CancellationToken);
        await adapter.Write("second", TestContext.CancellationToken);

        // assert
        File.Exists(backupFilename).Should().BeFalse();
    }

    [TestMethod]
    public async Task Write_WithBackupEnabled_NoExistingFile_DoesNotCreateBackupFile()
    {
        // arrange
        var filename = "text-backup-enabled-first-write-async.txt";
        var backupFilename = filename + ".bak";
        File.Delete(filename);
        File.Delete(backupFilename);
        var adapter = new TextFileAdapterAsync(filename, enableBackup: true);

        // act
        await adapter.Write("first", TestContext.CancellationToken);

        // assert
        File.Exists(backupFilename).Should().BeFalse();
    }

    [TestMethod]
    public async Task Write_WithBackupEnabled_ExistingFile_CreatesBackupWithPreviousContent()
    {
        // arrange
        var filename = "text-backup-enabled-second-write-async.txt";
        var backupFilename = filename + ".bak";
        File.Delete(filename);
        File.Delete(backupFilename);
        var adapter = new TextFileAdapterAsync(filename, enableBackup: true);

        // act
        await adapter.Write("first", TestContext.CancellationToken);
        await adapter.Write("second", TestContext.CancellationToken);

        // assert
        File.Exists(backupFilename).Should().BeTrue();
        (await File.ReadAllTextAsync(backupFilename, TestContext.CancellationToken)).Should().Be("first");
        (await File.ReadAllTextAsync(filename, TestContext.CancellationToken)).Should().Be("second");
    }

    [TestMethod]
    public async Task Read_WithBackupDisabled_MissingPrimaryFile_ReturnsNull()
    {
        // arrange
        var filename = "text-backup-disabled-missing-primary-async.txt";
        var backupFilename = filename + ".bak";
        File.Delete(filename);
        await File.WriteAllTextAsync(backupFilename, "backup-content", TestContext.CancellationToken);
        var adapter = new TextFileAdapterAsync(filename);

        // act
        var result = await adapter.Read(TestContext.CancellationToken);

        // assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task Read_WithBackupEnabled_MissingPrimaryFile_FallsBackToBackupContent()
    {
        // arrange
        var filename = "text-backup-enabled-missing-primary-async.txt";
        var backupFilename = filename + ".bak";
        File.Delete(filename);
        await File.WriteAllTextAsync(backupFilename, "backup-content", TestContext.CancellationToken);
        var adapter = new TextFileAdapterAsync(filename, enableBackup: true);

        // act
        var result = await adapter.Read(TestContext.CancellationToken);

        // assert
        result.Should().Be("backup-content");
    }

    [TestMethod]
    public async Task Read_WithBackupEnabled_PrimaryFileExists_DoesNotUseBackup()
    {
        // arrange
        var filename = "text-backup-enabled-primary-exists-async.txt";
        var backupFilename = filename + ".bak";
        await File.WriteAllTextAsync(filename, "primary-content", TestContext.CancellationToken);
        await File.WriteAllTextAsync(backupFilename, "backup-content", TestContext.CancellationToken);
        var adapter = new TextFileAdapterAsync(filename, enableBackup: true);

        // act
        var result = await adapter.Read(TestContext.CancellationToken);

        // assert
        result.Should().Be("primary-content");
    }

    [TestMethod]
    public async Task ReadBackup_WithNoBackupFile_ReturnsNull()
    {
        // arrange
        var filename = "text-backup-readbackup-missing-async.txt";
        var backupFilename = filename + ".bak";
        File.Delete(backupFilename);
        var adapter = new TextFileAdapterAsync(filename, enableBackup: true);

        // act
        var result = await adapter.ReadBackup(TestContext.CancellationToken);

        // assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task ReadBackup_WithExistingBackupFile_ReturnsBackupContent()
    {
        // arrange
        var filename = "text-backup-readbackup-existing-async.txt";
        var backupFilename = filename + ".bak";
        await File.WriteAllTextAsync(backupFilename, "backup-content", TestContext.CancellationToken);
        var adapter = new TextFileAdapterAsync(filename, enableBackup: true);

        // act
        var result = await adapter.ReadBackup(TestContext.CancellationToken);

        // assert
        result.Should().Be("backup-content");
    }

    public TestContext TestContext { get; set; } = default!;
}
