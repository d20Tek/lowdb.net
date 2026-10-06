using D20Tek.Blazor.BrowserStorage;
using D20Tek.Blazor.BrowserStorage.Testing;
using D20Tek.LowDb.Browser.Adapters;
using D20Tek.LowDb.UnitTests.Entities;
using D20Tek.LowDb.UnitTests.Fakes;
using FluentAssertions;

namespace D20Tek.LowDb.UnitTests.Browser;

[TestClass]
public class LocalStorageAdapterAsyncBackupTests
{
    private readonly ILocalStorageService storage = new InMemoryLocalStorageService();

    [TestMethod]
    public async Task Write_WithBackupEnabled_ExistingKey_CreatesBackupKey()
    {
        // arrange
        var keyname = "local-backup-write-test";
        var adapter = new LocalStorageAdapterAsync<TestDocument>(keyname, storage, enableBackup: true);
        var db = new LowDbAsync<TestDocument>(adapter);

        // act
        await db.Update(x => x.Entities.Add(TestEntityFactory.Create(1)), token: TestContext.CancellationToken);
        await db.Update(x => x.Entities.Add(TestEntityFactory.Create(2)), token: TestContext.CancellationToken);

        // assert
        var backupResult = await storage.GetAsync<TestDocument>(keyname + ".bak", TestContext.CancellationToken);
        backupResult.IsSuccess.Should().BeTrue();
        backupResult.Value!.Entities.Should().ContainSingle(x => x.Id == 1);
    }

    [TestMethod]
    public async Task Write_WithBackupDisabled_DoesNotCreateBackupKey()
    {
        // arrange
        var keyname = "local-backup-disabled-write-test";
        var adapter = new LocalStorageAdapterAsync<TestDocument>(keyname, storage);
        var db = new LowDbAsync<TestDocument>(adapter);

        // act
        await db.Update(x => x.Entities.Add(TestEntityFactory.Create(1)), token: TestContext.CancellationToken);
        await db.Update(x => x.Entities.Add(TestEntityFactory.Create(2)), token: TestContext.CancellationToken);

        // assert
        var backupResult = await storage.GetAsync<TestDocument>(keyname + ".bak", TestContext.CancellationToken);
        backupResult.IsSuccess.Should().BeFalse();
    }

    [TestMethod]
    public async Task Read_WithBackupEnabled_MissingPrimaryKey_FallsBackToBackupContent()
    {
        // arrange
        var keyname = "local-backup-fallback-test";
        var backupKeyname = keyname + ".bak";
        var backupDoc = new TestDocument();
        backupDoc.Entities.Add(TestEntityFactory.Create(1));
        await storage.SetAsync(backupKeyname, backupDoc, TestContext.CancellationToken);

        var adapter = new LocalStorageAdapterAsync<TestDocument>(keyname, storage, enableBackup: true);

        // act
        var result = await adapter.Read(TestContext.CancellationToken);

        // assert
        result.Should().NotBeNull();
        result!.Entities.Should().ContainSingle(x => x.Id == 1);
    }

    [TestMethod]
    public async Task Read_WithBackupDisabled_MissingPrimaryKey_ReturnsNull()
    {
        // arrange
        var keyname = "local-backup-disabled-fallback-test";
        var backupKeyname = keyname + ".bak";
        var backupDoc = new TestDocument();
        backupDoc.Entities.Add(TestEntityFactory.Create(1));
        await storage.SetAsync(backupKeyname, backupDoc, TestContext.CancellationToken);

        var adapter = new LocalStorageAdapterAsync<TestDocument>(keyname, storage);

        // act
        var result = await adapter.Read(TestContext.CancellationToken);

        // assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task Read_WithBackupEnabled_PrimaryKeyExists_DoesNotUseBackup()
    {
        // arrange
        var keyname = "local-backup-primary-exists-test";
        var backupKeyname = keyname + ".bak";
        var primaryDoc = new TestDocument();
        primaryDoc.Entities.Add(TestEntityFactory.Create(1));
        await storage.SetAsync(keyname, primaryDoc, TestContext.CancellationToken);

        var backupDoc = new TestDocument();
        backupDoc.Entities.Add(TestEntityFactory.Create(2));
        await storage.SetAsync(backupKeyname, backupDoc, TestContext.CancellationToken);

        var adapter = new LocalStorageAdapterAsync<TestDocument>(keyname, storage, enableBackup: true);

        // act
        var result = await adapter.Read(TestContext.CancellationToken);

        // assert
        result.Should().NotBeNull();
        result!.Entities.Should().ContainSingle(x => x.Id == 1);
    }

    [TestMethod]
    public async Task Read_WithBackupEnabled_MissingPrimaryAndBackupKey_ReturnsNull()
    {
        // arrange
        var keyname = "local-backup-no-backup-test";
        var adapter = new LocalStorageAdapterAsync<TestDocument>(keyname, storage, enableBackup: true);

        // act
        var result = await adapter.Read(TestContext.CancellationToken);

        // assert
        result.Should().BeNull();
    }

    public TestContext TestContext { get; set; } = default!;
}
