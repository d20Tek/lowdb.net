using D20Tek.LowDb.Adapters;
using D20Tek.LowDb.UnitTests.Entities;
using D20Tek.LowDb.UnitTests.Fakes;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.LowDb.UnitTests;

[TestClass]
public class LowDbAsyncChangedEventTests
{
    [TestMethod]
    public async Task Write_WithSubscriber_RaisesChangedEvent()
    {
        // arrange
        using var db = new LowDbAsync<TestDocument>(new MemoryStorageAdapterAsync<TestDocument>());
        var raised = 0;
        db.Changed += (_, _) => raised++;

        // act
        await db.Write(TestContext.CancellationToken);

        // assert
        raised.Should().Be(1);
    }

    [TestMethod]
    public async Task Write_WithNoSubscriber_DoesNotThrow()
    {
        // arrange
        using var db = new LowDbAsync<TestDocument>(new MemoryStorageAdapterAsync<TestDocument>());

        // act
        var act = async () => await db.Write(TestContext.CancellationToken);

        // assert
        await act.Should().NotThrowAsync();
    }

    [TestMethod]
    public async Task Write_WithSubscriber_PassesPersistedDocumentInEventArgs()
    {
        // arrange
        using var db = new LowDbAsync<TestDocument>(new MemoryStorageAdapterAsync<TestDocument>());
        TestDocument? received = null;
        db.Changed += (_, e) => received = e.Document;
        await db.Update(
            x => x.Entities.Add(TestEntityFactory.Create(1)),
            autoSave: false,
            token: TestContext.CancellationToken);

        // act
        await db.Write(TestContext.CancellationToken);

        // assert
        received.Should().BeSameAs(await db.Get(TestContext.CancellationToken));
        received!.Entities.Should().ContainSingle(x => x.Id == 1);
    }

    [TestMethod]
    public async Task Write_WithSubscriber_PassesDatabaseAsSender()
    {
        // arrange
        using var db = new LowDbAsync<TestDocument>(new MemoryStorageAdapterAsync<TestDocument>());
        object? sender = null;
        db.Changed += (s, _) => sender = s;

        // act
        await db.Write(TestContext.CancellationToken);

        // assert
        sender.Should().BeSameAs(db);
    }

    [TestMethod]
    public async Task Update_WithAutoSaveTrue_RaisesChangedEvent()
    {
        // arrange
        using var db = new LowDbAsync<TestDocument>(new MemoryStorageAdapterAsync<TestDocument>());
        var raised = 0;
        db.Changed += (_, _) => raised++;

        // act
        await db.Update(x => x.Entities.Add(TestEntityFactory.Create(1)), token: TestContext.CancellationToken);

        // assert
        raised.Should().Be(1);
    }

    [TestMethod]
    public async Task Update_WithAutoSaveFalse_DoesNotRaiseChangedEvent()
    {
        // arrange
        using var db = new LowDbAsync<TestDocument>(new MemoryStorageAdapterAsync<TestDocument>());
        var raised = 0;
        db.Changed += [ExcludeFromCodeCoverage](_, _) => raised++;

        // act
        await db.Update(
            x => x.Entities.Add(TestEntityFactory.Create(1)),
            autoSave: false,
            token: TestContext.CancellationToken);

        // assert
        raised.Should().Be(0);
    }

    [TestMethod]
    public async Task Read_WithSubscriber_DoesNotRaiseChangedEvent()
    {
        // arrange
        using var db = new LowDbAsync<TestDocument>(new MemoryStorageAdapterAsync<TestDocument>());
        var raised = 0;
        db.Changed += [ExcludeFromCodeCoverage](_, _) => raised++;

        // act
        await db.Read(TestContext.CancellationToken);

        // assert
        raised.Should().Be(0);
    }

    [TestMethod]
    public async Task Get_WithSubscriber_DoesNotRaiseChangedEvent()
    {
        // arrange
        using var db = new LowDbAsync<TestDocument>(new MemoryStorageAdapterAsync<TestDocument>());
        var raised = 0;
        db.Changed += [ExcludeFromCodeCoverage](_, _) => raised++;

        // act
        _ = await db.Get(TestContext.CancellationToken);

        // assert
        raised.Should().Be(0);
    }

    [TestMethod]
    public async Task Changed_WithMultipleSubscribers_NotifiesAllSubscribers()
    {
        // arrange
        using var db = new LowDbAsync<TestDocument>(new MemoryStorageAdapterAsync<TestDocument>());
        var first = 0;
        var second = 0;
        db.Changed += (_, _) => first++;
        db.Changed += (_, _) => second++;

        // act
        await db.Write(TestContext.CancellationToken);

        // assert
        first.Should().Be(1);
        second.Should().Be(1);
    }

    [TestMethod]
    public async Task Changed_AfterUnsubscribe_DoesNotNotifyRemovedHandler()
    {
        // arrange
        using var db = new LowDbAsync<TestDocument>(new MemoryStorageAdapterAsync<TestDocument>());
        var raised = 0;

        [ExcludeFromCodeCoverage]
        void Handler(object? sender, DocumentChangedEventArgs<TestDocument> e) => raised++;
        db.Changed += Handler;
        db.Changed -= Handler;

        // act
        await db.Write(TestContext.CancellationToken);

        // assert
        raised.Should().Be(0);
    }

    [TestMethod]
    public async Task Changed_HandlerCallsBackIntoDatabase_DoesNotDeadlock()
    {
        // arrange
        using var db = new LowDbAsync<TestDocument>(new MemoryStorageAdapterAsync<TestDocument>());
        TestDocument? fromHandler = null;
        db.Changed += (_, _) => fromHandler = db.Get(TestContext.CancellationToken).GetAwaiter().GetResult();

        // act
        await db.Write(TestContext.CancellationToken);

        // assert
        fromHandler.Should().NotBeNull();
    }

    [TestMethod]
    public async Task Changed_WithMultipleWrites_RaisesEventPerWrite()
    {
        // arrange
        using var db = new LowDbAsync<TestDocument>(new MemoryStorageAdapterAsync<TestDocument>());
        var raised = 0;
        db.Changed += (_, _) => raised++;

        // act
        await db.Update(x => x.Entities.Add(TestEntityFactory.Create(1)), token: TestContext.CancellationToken);
        await db.Update(x => x.Entities.Add(TestEntityFactory.Create(2)), token: TestContext.CancellationToken);
        await db.Write(TestContext.CancellationToken);

        // assert
        raised.Should().Be(3);
    }

    public TestContext TestContext { get; set; } = default!;
}
