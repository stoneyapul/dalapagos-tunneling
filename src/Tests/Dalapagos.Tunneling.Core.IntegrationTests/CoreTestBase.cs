namespace Dalapagos.Tunneling.Core.IntegrationTests;

using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

public abstract class CoreTestBase(ITestOutputHelper testOutputHelper, CoreTestFixture fixture)
    : TestBed<CoreTestFixture>(testOutputHelper, fixture)
{
    protected static ILogger<T> GetLogger<T>() => Substitute.For<ILogger<T>>();

    protected T GetNonNullService<T>()
    {
        var service = _fixture.GetService<T>(_testOutputHelper);
        Assert.NotNull(service);
        return service;
    }
}