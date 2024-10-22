//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using Blazored.LocalStorage;
using D20Tek.LowDb.Browser;
using D20Tek.LowDb.UnitTests.Entities;
using D20Tek.LowDb.UnitTests.Fakes;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace D20Tek.LowDb.UnitTests.Browser;

[TestClass]
public class DependencyInjectionTests
{
    [TestMethod]
    public void AddLocalLowDb_AddsDbAndLocalStorageAdapter()
    {
        // arrange
        var keyname = "test-key-1";
        var services = CreateServiceCollection();

        // act
        services.AddLocalLowDb<TestDocument>(keyname);

        // assert
        services.Any(x => x.ServiceType == typeof(LowDb<TestDocument>)).Should().BeTrue();

        // act on service provider
        var provider = services.BuildServiceProvider();

        // assert
        var lowdb = provider.GetService<LowDb<TestDocument>>();
        lowdb.Should().NotBeNull();
    }

    [TestMethod]
    public void AddLowDbAsync_AddsDbAndJsonFileAdapter()
    {
        // arrange
        var keyname = "test-key-2";
        var services = CreateServiceCollection();

        // act
        services.AddLocalLowDbAsync<TestDocument>(keyname, ServiceLifetime.Singleton);

        // assert
        services.Any(x => x.ServiceType == typeof(LowDbAsync<TestDocument>)).Should().BeTrue();

        // act on service provider
        var provider = services.BuildServiceProvider();

        // assert
        var lowdb = provider.GetService<LowDbAsync<TestDocument>>();
        lowdb.Should().NotBeNull();
    }

    private static IServiceCollection CreateServiceCollection() =>
        new ServiceCollection()
            .AddSingleton<IJSRuntime, FakeJSRuntime>()
            .AddBlazoredLocalStorage();
}
