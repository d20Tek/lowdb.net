//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.LowDb.UnitTests.Entities;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace D20Tek.LowDb.UnitTests;

[TestClass]
public class DependencyInjectionTests
{
    [TestMethod]
    public void AddLowDb_AddsDbAndJsonFileAdapter()
    {
        // arrange
        var filename = Path.GetTempFileName();
        var services = new ServiceCollection();

        // act
        services.AddLowDb<TestDocument>(filename);

        // assert
        services.Any(x => x.ServiceType == typeof(LowDb<TestDocument>)).Should().BeTrue();

        // act on service provider
        var provider = services.BuildServiceProvider();

        // assert
        var lowdb = provider.GetService<LowDb<TestDocument>>();
        lowdb.Should().NotBeNull();
    }
}
