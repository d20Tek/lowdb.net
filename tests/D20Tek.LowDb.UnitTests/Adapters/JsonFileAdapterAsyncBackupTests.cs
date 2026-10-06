using D20Tek.LowDb.Adapters;
using D20Tek.LowDb.UnitTests.Entities;
using D20Tek.LowDb.UnitTests.Fakes;
using FluentAssertions;

namespace D20Tek.LowDb.UnitTests.Adapters;

[TestClass]
public class JsonFileAdapterAsyncBackupTests
{
    [TestMethod]
    public async Task Write_WithBackupEnabled_ExistingFile_CreatesBackupFile()
    {
        // arrange
        var filename = "json-backup-write-test-async.json";
        var backupFilename = filename + ".bak";
        File.Delete(filename);
        File.Delete(backupFilename);
        var adapter = new JsonFileAdapterAsync<TestDocument>(filename, enableBackup: true);
        var db = new LowDbAsync<TestDocument>(adapter);

        // act
        await db.Update(x => x.Entities.Add(TestEntityFactory.Create(1)), token: TestContext.CancellationToken);
        await db.Update(x => x.Entities.Add(TestEntityFactory.Create(2)), token: TestContext.CancellationToken);

        // assert
        File.Exists(backupFilename).Should().BeTrue();
    }

    [TestMethod]
    public async Task Read_WithBackupDisabled_CorruptPrimaryFile_ThrowsJsonException()
    {
        // arrange
        var filename = "json-backup-disabled-corrupt-test-async.json";
        await File.WriteAllTextAsync(filename, "{ not-valid-json", TestContext.CancellationToken);
        var adapter = new JsonFileAdapterAsync<TestDocument>(filename);

        // act & assert
        await Assert.ThrowsExactlyAsync<System.Text.Json.JsonException>(Act);

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        Task Act() => adapter.Read(TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task Read_WithBackupEnabled_CorruptPrimaryFile_FallsBackToBackup()
    {
        // arrange
        var filename = "json-backup-enabled-corrupt-test-async.json";
        var backupFilename = filename + ".bak";
        var adapter = new JsonFileAdapterAsync<TestDocument>(filename, enableBackup: true);
        var db = new LowDbAsync<TestDocument>(adapter);
        File.Delete(filename);
        File.Delete(backupFilename);

        await db.Update(x => x.Entities.Add(TestEntityFactory.Create(1)), token: TestContext.CancellationToken);
        await db.Update(x => x.Entities.Add(TestEntityFactory.Create(2)), token: TestContext.CancellationToken);

        // corrupt the primary file; the backup still holds the previous good content
        await File.WriteAllTextAsync(filename, "{ not-valid-json", TestContext.CancellationToken);

        // act
        var result = await adapter.Read(TestContext.CancellationToken);

        // assert
        result.Should().NotBeNull();
        result!.Entities.Should().ContainSingle(x => x.Id == 1);
    }

    [TestMethod]
    public async Task Read_WithBackupEnabled_CorruptPrimaryFileAndNoBackup_ThrowsJsonException()
    {
        // arrange
        var filename = "json-backup-enabled-corrupt-no-backup-test-async.json";
        var backupFilename = filename + ".bak";
        File.Delete(backupFilename);
        await File.WriteAllTextAsync(filename, "{ not-valid-json", TestContext.CancellationToken);
        var adapter = new JsonFileAdapterAsync<TestDocument>(filename, enableBackup: true);

        // act & assert
        await Assert.ThrowsExactlyAsync<System.Text.Json.JsonException>(Act);

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        Task Act() => adapter.Read(TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task Read_WithBackupEnabled_CorruptPrimaryFileAndEmptyBackup_ThrowsJsonException()
    {
        // arrange
        var filename = "json-backup-enabled-corrupt-empty-backup-test-async-2.json";
        var backupFilename = filename + ".bak";
        await File.WriteAllTextAsync(backupFilename, string.Empty, TestContext.CancellationToken);
        await File.WriteAllTextAsync(filename, "{ not-valid-json", TestContext.CancellationToken);
        var adapter = new JsonFileAdapterAsync<TestDocument>(filename, enableBackup: true);

        // act & assert
        await Assert.ThrowsExactlyAsync<System.Text.Json.JsonException>(Act);

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        Task Act() => adapter.Read(TestContext.CancellationToken);
    }

    public TestContext TestContext { get; set; } = default!;
}
