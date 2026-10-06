using D20Tek.LowDb.Adapters;
using FluentAssertions;

namespace D20Tek.LowDb.UnitTests.Adapters;

[TestClass]
public class TextFileAdapterBackupTests
{
    [TestMethod]
    public void Write_WithBackupDisabled_DoesNotCreateBackupFile()
    {
        // arrange
        var filename = "text-backup-disabled.txt";
        var backupFilename = filename + ".bak";
        File.Delete(filename);
        File.Delete(backupFilename);
        var adapter = new TextFileAdapter(filename);

        // act
        adapter.Write("first");
        adapter.Write("second");

        // assert
        File.Exists(backupFilename).Should().BeFalse();
    }

    [TestMethod]
    public void Write_WithBackupEnabled_NoExistingFile_DoesNotCreateBackupFile()
    {
        // arrange
        var filename = "text-backup-enabled-first-write.txt";
        var backupFilename = filename + ".bak";
        File.Delete(filename);
        File.Delete(backupFilename);
        var adapter = new TextFileAdapter(filename, enableBackup: true);

        // act
        adapter.Write("first");

        // assert
        File.Exists(backupFilename).Should().BeFalse();
    }

    [TestMethod]
    public void Write_WithBackupEnabled_ExistingFile_CreatesBackupWithPreviousContent()
    {
        // arrange
        var filename = "text-backup-enabled-second-write.txt";
        var backupFilename = filename + ".bak";
        File.Delete(filename);
        File.Delete(backupFilename);
        var adapter = new TextFileAdapter(filename, enableBackup: true);

        // act
        adapter.Write("first");
        adapter.Write("second");

        // assert
        File.Exists(backupFilename).Should().BeTrue();
        File.ReadAllText(backupFilename).Should().Be("first");
        File.ReadAllText(filename).Should().Be("second");
    }

    [TestMethod]
    public void Read_WithBackupDisabled_MissingPrimaryFile_ReturnsNull()
    {
        // arrange
        var filename = "text-backup-disabled-missing-primary.txt";
        var backupFilename = filename + ".bak";
        File.Delete(filename);
        File.WriteAllText(backupFilename, "backup-content");
        var adapter = new TextFileAdapter(filename);

        // act
        var result = adapter.Read();

        // assert
        result.Should().BeNull();
    }

    [TestMethod]
    public void Read_WithBackupEnabled_MissingPrimaryFile_FallsBackToBackupContent()
    {
        // arrange
        var filename = "text-backup-enabled-missing-primary.txt";
        var backupFilename = filename + ".bak";
        File.Delete(filename);
        File.WriteAllText(backupFilename, "backup-content");
        var adapter = new TextFileAdapter(filename, enableBackup: true);

        // act
        var result = adapter.Read();

        // assert
        result.Should().Be("backup-content");
    }

    [TestMethod]
    public void Read_WithBackupEnabled_PrimaryFileExists_DoesNotUseBackup()
    {
        // arrange
        var filename = "text-backup-enabled-primary-exists.txt";
        var backupFilename = filename + ".bak";
        File.WriteAllText(filename, "primary-content");
        File.WriteAllText(backupFilename, "backup-content");
        var adapter = new TextFileAdapter(filename, enableBackup: true);

        // act
        var result = adapter.Read();

        // assert
        result.Should().Be("primary-content");
    }

    [TestMethod]
    public void ReadBackup_WithNoBackupFile_ReturnsNull()
    {
        // arrange
        var filename = "text-backup-readbackup-missing.txt";
        var backupFilename = filename + ".bak";
        File.Delete(backupFilename);
        var adapter = new TextFileAdapter(filename, enableBackup: true);

        // act
        var result = adapter.ReadBackup();

        // assert
        result.Should().BeNull();
    }

    [TestMethod]
    public void ReadBackup_WithExistingBackupFile_ReturnsBackupContent()
    {
        // arrange
        var filename = "text-backup-readbackup-existing.txt";
        var backupFilename = filename + ".bak";
        File.WriteAllText(backupFilename, "backup-content");
        var adapter = new TextFileAdapter(filename, enableBackup: true);

        // act
        var result = adapter.ReadBackup();

        // assert
        result.Should().Be("backup-content");
    }
}
