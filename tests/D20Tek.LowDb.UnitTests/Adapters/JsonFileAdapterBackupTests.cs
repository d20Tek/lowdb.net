using D20Tek.LowDb.Adapters;
using D20Tek.LowDb.UnitTests.Entities;
using D20Tek.LowDb.UnitTests.Fakes;
using FluentAssertions;

namespace D20Tek.LowDb.UnitTests.Adapters;

[TestClass]
public class JsonFileAdapterBackupTests
{
    [TestMethod]
    public void Write_WithBackupEnabled_ExistingFile_CreatesBackupFile()
    {
        // arrange
        var filename = "json-backup-write-test.json";
        var backupFilename = filename + ".bak";
        File.Delete(filename);
        File.Delete(backupFilename);
        var adapter = new JsonFileAdapter<TestDocument>(filename, enableBackup: true);
        var db = new LowDb<TestDocument>(adapter);

        // act
        db.Update(x => x.Entities.Add(TestEntityFactory.Create(1)));
        db.Update(x => x.Entities.Add(TestEntityFactory.Create(2)));

        // assert
        File.Exists(backupFilename).Should().BeTrue();
    }

    [TestMethod]
    public void Read_WithBackupDisabled_CorruptPrimaryFile_ThrowsJsonException()
    {
        // arrange
        var filename = "json-backup-disabled-corrupt-test.json";
        File.WriteAllText(filename, "{ not-valid-json");
        var adapter = new JsonFileAdapter<TestDocument>(filename);

        // act & assert
        Act().Should().Throw<System.Text.Json.JsonException>();

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        Action Act() => () => adapter.Read();
    }

    [TestMethod]
    public void Read_WithBackupEnabled_CorruptPrimaryFile_FallsBackToBackup()
    {
        // arrange
        var filename = "json-backup-enabled-corrupt-test.json";
        var backupFilename = filename + ".bak";
        var adapter = new JsonFileAdapter<TestDocument>(filename, enableBackup: true);
        var db = new LowDb<TestDocument>(adapter);
        File.Delete(filename);
        File.Delete(backupFilename);

        db.Update(x => x.Entities.Add(TestEntityFactory.Create(1)));
        db.Update(x => x.Entities.Add(TestEntityFactory.Create(2)));

        // corrupt the primary file; the backup still holds the previous good content
        File.WriteAllText(filename, "{ not-valid-json");

        // act
        var result = adapter.Read();

        // assert
        result.Should().NotBeNull();
        result!.Entities.Should().ContainSingle(x => x.Id == 1);
    }

    [TestMethod]
    public void Read_WithBackupEnabled_CorruptPrimaryFileAndNoBackup_ThrowsJsonException()
    {
        // arrange
        var filename = "json-backup-enabled-corrupt-no-backup-test.json";
        var backupFilename = filename + ".bak";
        File.Delete(backupFilename);
        File.WriteAllText(filename, "{ not-valid-json");
        var adapter = new JsonFileAdapter<TestDocument>(filename, enableBackup: true);

        // act & assert
        Act().Should().Throw<System.Text.Json.JsonException>();

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        Action Act() => () => adapter.Read();
    }

    [TestMethod]
    public void Read_WithBackupEnabled_CorruptPrimaryFileAndEmptyBackup_ThrowsJsonException()
    {
        // arrange
        var filename = "json-backup-enabled-corrupt-empty-backup-test-2.json";
        var backupFilename = filename + ".bak";
        File.WriteAllText(backupFilename, string.Empty);
        File.WriteAllText(filename, "{ not-valid-json");
        var adapter = new JsonFileAdapter<TestDocument>(filename, enableBackup: true);

        // act & assert
        Act().Should().Throw<System.Text.Json.JsonException>();

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        Action Act() => () => adapter.Read();
    }
}
