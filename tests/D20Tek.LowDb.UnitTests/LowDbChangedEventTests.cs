using D20Tek.LowDb.Adapters;
using D20Tek.LowDb.UnitTests.Entities;
using D20Tek.LowDb.UnitTests.Fakes;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.LowDb.UnitTests;

[TestClass]
public class LowDbChangedEventTests
{
    [TestMethod]
    public void Write_WithSubscriber_RaisesChangedEvent()
    {
        // arrange
        var db = new LowDb<TestDocument>(new MemoryStorageAdapter<TestDocument>());
        var raised = 0;
        db.Changed += (_, _) => raised++;

        // act
        db.Write();

        // assert
        raised.Should().Be(1);
    }

    [TestMethod]
    public void Write_WithNoSubscriber_DoesNotThrow()
    {
        // arrange
        var db = new LowDb<TestDocument>(new MemoryStorageAdapter<TestDocument>());

        // act
        var act = () => db.Write();

        // assert
        act.Should().NotThrow();
    }

    [TestMethod]
    public void Write_WithSubscriber_PassesPersistedDocumentInEventArgs()
    {
        // arrange
        var db = new LowDb<TestDocument>(new MemoryStorageAdapter<TestDocument>());
        TestDocument? received = null;
        db.Changed += (_, e) => received = e.Document;
        db.Update(x => x.Entities.Add(TestEntityFactory.Create(1)), autoSave: false);

        // act
        db.Write();

        // assert
        received.Should().BeSameAs(db.Get());
        received!.Entities.Should().ContainSingle(x => x.Id == 1);
    }

    [TestMethod]
    public void Write_WithSubscriber_PassesDatabaseAsSender()
    {
        // arrange
        var db = new LowDb<TestDocument>(new MemoryStorageAdapter<TestDocument>());
        object? sender = null;
        db.Changed += (s, _) => sender = s;

        // act
        db.Write();

        // assert
        sender.Should().BeSameAs(db);
    }

    [TestMethod]
    public void Update_WithAutoSaveTrue_RaisesChangedEvent()
    {
        // arrange
        var db = new LowDb<TestDocument>(new MemoryStorageAdapter<TestDocument>());
        var raised = 0;
        db.Changed += (_, _) => raised++;

        // act
        db.Update(x => x.Entities.Add(TestEntityFactory.Create(1)));

        // assert
        raised.Should().Be(1);
    }

    [TestMethod]
    public void Update_WithAutoSaveFalse_DoesNotRaiseChangedEvent()
    {
        // arrange
        var db = new LowDb<TestDocument>(new MemoryStorageAdapter<TestDocument>());
        var raised = 0;
        db.Changed += [ExcludeFromCodeCoverage](_, _) => raised++;

        // act
        db.Update(x => x.Entities.Add(TestEntityFactory.Create(1)), autoSave: false);

        // assert
        raised.Should().Be(0);
    }

    [TestMethod]
    public void Read_WithSubscriber_DoesNotRaiseChangedEvent()
    {
        // arrange
        var db = new LowDb<TestDocument>(new MemoryStorageAdapter<TestDocument>());
        var raised = 0;
        db.Changed += [ExcludeFromCodeCoverage](_, _) => raised++;

        // act
        db.Read();

        // assert
        raised.Should().Be(0);
    }

    [TestMethod]
    public void Get_WithSubscriber_DoesNotRaiseChangedEvent()
    {
        // arrange
        var db = new LowDb<TestDocument>(new MemoryStorageAdapter<TestDocument>());
        var raised = 0;
        db.Changed += [ExcludeFromCodeCoverage](_, _) => raised++;

        // act
        _ = db.Get();

        // assert
        raised.Should().Be(0);
    }

    [TestMethod]
    public void Changed_WithMultipleSubscribers_NotifiesAllSubscribers()
    {
        // arrange
        var db = new LowDb<TestDocument>(new MemoryStorageAdapter<TestDocument>());
        var first = 0;
        var second = 0;
        db.Changed += (_, _) => first++;
        db.Changed += (_, _) => second++;

        // act
        db.Write();

        // assert
        first.Should().Be(1);
        second.Should().Be(1);
    }

    [TestMethod]
    public void Changed_AfterUnsubscribe_DoesNotNotifyRemovedHandler()
    {
        // arrange
        var db = new LowDb<TestDocument>(new MemoryStorageAdapter<TestDocument>());
        var raised = 0;

        [ExcludeFromCodeCoverage]
        void Handler(object? sender, DocumentChangedEventArgs<TestDocument> e) => raised++;
        db.Changed += Handler;
        db.Changed -= Handler;

        // act
        db.Write();

        // assert
        raised.Should().Be(0);
    }

    [TestMethod]
    public void Changed_HandlerCallsBackIntoDatabase_DoesNotDeadlock()
    {
        // arrange
        var db = new LowDb<TestDocument>(new MemoryStorageAdapter<TestDocument>());
        TestDocument? fromHandler = null;
        db.Changed += (_, _) => fromHandler = db.Get();

        // act
        db.Write();

        // assert
        fromHandler.Should().NotBeNull();
    }

    [TestMethod]
    public void Changed_WithMultipleWrites_RaisesEventPerWrite()
    {
        // arrange
        var db = new LowDb<TestDocument>(new MemoryStorageAdapter<TestDocument>());
        var raised = 0;
        db.Changed += (_, _) => raised++;

        // act
        db.Update(x => x.Entities.Add(TestEntityFactory.Create(1)));
        db.Update(x => x.Entities.Add(TestEntityFactory.Create(2)));
        db.Write();

        // assert
        raised.Should().Be(3);
    }
}
