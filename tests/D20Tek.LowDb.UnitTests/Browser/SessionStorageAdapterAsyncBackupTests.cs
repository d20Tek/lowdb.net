using D20Tek.Blazor.BrowserStorage;
using D20Tek.Blazor.BrowserStorage.Testing;
using D20Tek.LowDb.Browser.Adapters;
using D20Tek.LowDb.UnitTests.Entities;
using D20Tek.LowDb.UnitTests.Fakes;
using FluentAssertions;

namespace D20Tek.LowDb.UnitTests.Browser;

[TestClass]
public class SessionStorageAdapterAsyncBackupTests
{
    private readonly ISessionStorageService _storage = new InMemorySessionStorageService();

    [TestMethod]
    public async Task Write_WithBackupEnabled_ExistingKey_CreatesBackupKey()
    {
        // arrange
        var keyname = "session-backup-write-test";
        var adapter = new SessionStorageAdapterAsync<TestDocument>(keyname, _storage, enableBackup: true);
        var db = new LowDbAsync<TestDocument>(adapter);

        // act
        await db.Update(x => x.Entities.Add(TestEntityFactory.Create(1)), token: TestContext.CancellationToken);
        await db.Update(x => x.Entities.Add(TestEntityFactory.Create(2)), token: TestContext.CancellationToken);

        // assert
        var backupResult = await _storage.GetAsync<TestDocument>(keyname + ".bak", TestContext.CancellationToken);
        backupResult.IsSuccess.Should().BeTrue();
        backupResult.Value!.Entities.Should().ContainSingle(x => x.Id == 1);
    }

    [TestMethod]
    public async Task Write_WithBackupDisabled_DoesNotCreateBackupKey()
    {
        // arrange
        var keyname = "session-backup-disabled-write-test";
        var adapter = new SessionStorageAdapterAsync<TestDocument>(keyname, _storage);
        var db = new LowDbAsync<TestDocument>(adapter);

        // act
        await db.Update(x => x.Entities.Add(TestEntityFactory.Create(1)), token: TestContext.CancellationToken);
        await db.Update(x => x.Entities.Add(TestEntityFactory.Create(2)), token: TestContext.CancellationToken);

        // assert
        var backupResult = await _storage.GetAsync<TestDocument>(keyname + ".bak", TestContext.CancellationToken);
        backupResult.IsSuccess.Should().BeFalse();
    }

    [TestMethod]
    public async Task Read_WithBackupEnabled_MissingPrimaryKey_FallsBackToBackupContent()
    {
        // arrange
        var keyname = "session-backup-fallback-test";
        var backupKeyname = keyname + ".bak";
        var backupDoc = new TestDocument();
        backupDoc.Entities.Add(TestEntityFactory.Create(1));
        await _storage.SetAsync(backupKeyname, backupDoc, TestContext.CancellationToken);

        var adapter = new SessionStorageAdapterAsync<TestDocument>(keyname, _storage, enableBackup: true);

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
        var keyname = "session-backup-disabled-fallback-test";
        var backupKeyname = keyname + ".bak";
        var backupDoc = new TestDocument();
        backupDoc.Entities.Add(TestEntityFactory.Create(1));
        await _storage.SetAsync(backupKeyname, backupDoc, TestContext.CancellationToken);

        var adapter = new SessionStorageAdapterAsync<TestDocument>(keyname, _storage);

        // act
        var result = await adapter.Read(TestContext.CancellationToken);

        // assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task Read_WithBackupEnabled_PrimaryKeyExists_DoesNotUseBackup()
    {
        // arrange
        var keyname = "session-backup-primary-exists-test";
        var backupKeyname = keyname + ".bak";
        var primaryDoc = new TestDocument();
        primaryDoc.Entities.Add(TestEntityFactory.Create(1));
        await _storage.SetAsync(keyname, primaryDoc, TestContext.CancellationToken);

        var backupDoc = new TestDocument();
        backupDoc.Entities.Add(TestEntityFactory.Create(2));
        await _storage.SetAsync(backupKeyname, backupDoc, TestContext.CancellationToken);

        var adapter = new SessionStorageAdapterAsync<TestDocument>(keyname, _storage, enableBackup: true);

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
        var keyname = "session-backup-no-backup-test";
        var adapter = new SessionStorageAdapterAsync<TestDocument>(keyname, _storage, enableBackup: true);

        // act
        var result = await adapter.Read(TestContext.CancellationToken);

        // assert
        result.Should().BeNull();
    }

    public TestContext TestContext { get; set; } = default!;
}
