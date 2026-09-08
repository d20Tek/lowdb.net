using D20Tek.Blazor.BrowserStorage.Testing;
using D20Tek.LowDb.Browser;
using D20Tek.LowDb.UnitTests.Entities;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace D20Tek.LowDb.UnitTests.Browser;

[TestClass]
public class DependencyInjectionTests
{
    [TestMethod]
    public void AddLocalLowDbAsync_AddsDbAndStorageAdapter()
    {
        // arrange
        var keyname = "test-key-2";
        var services = new ServiceCollection();

        // act
        services.AddLocalLowDbAsync<TestDocument>(keyname, ServiceLifetime.Singleton);
        services.ReplaceWithInMemoryBrowserStorage();

        // assert
        services.Any(x => x.ServiceType == typeof(LowDbAsync<TestDocument>)).Should().BeTrue();

        // act on service provider
        var provider = services.BuildServiceProvider();

        // assert
        var lowdb = provider.GetService<LowDbAsync<TestDocument>>();
        lowdb.Should().NotBeNull();
    }

    [TestMethod]
    public void AddSessionLowDbAsync_AddsDbAndStorageAdapter()
    {
        // arrange
        var keyname = "test-key-2";
        var services = new ServiceCollection();

        // act
        services.AddSessionLowDbAsync<TestDocument>(keyname, ServiceLifetime.Singleton);
        services.ReplaceWithInMemoryBrowserStorage();

        // assert
        services.Any(x => x.ServiceType == typeof(LowDbAsync<TestDocument>)).Should().BeTrue();

        // act on service provider
        var provider = services.BuildServiceProvider();

        // assert
        var lowdb = provider.GetService<LowDbAsync<TestDocument>>();
        lowdb.Should().NotBeNull();
    }
}
