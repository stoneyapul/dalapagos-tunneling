namespace Dalapagos.Tunneling.Core.IntegrationTests;

using Commands;
using Dalapagos.Tunneling.Core.Model;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;
using Xunit.Microsoft.DependencyInjection.Attributes;

[TestCaseOrderer(
    "Xunit.Microsoft.DependencyInjection.TestsOrder.TestPriorityOrderer",
    "Xunit.Microsoft.DependencyInjection"
)]
public class AddDeviceGroupTests(ITestOutputHelper testOutputHelper, CoreTestFixture fixture)
    : TestBed<CoreTestFixture>(testOutputHelper, fixture)
{
    private readonly Guid _organizationId = new("ce80cca7-2591-43c5-a721-442a610d814b");
    private readonly string _organizationName = "Acme Rockets";
    private readonly Guid _deviceGroupId = new("cd80cca7-2591-43c5-a721-442a710d812b");
    private readonly string _deviceGroupName = "Western Region";
    private readonly ServerLocation _serverLocation = ServerLocation.West;

    [Fact, TestOrder(1)]
    [Trait("Category", "Integration")]    
    public async Task AddOrganizationCommandPassAsync()
    {
        var cts = new CancellationTokenSource();
        var repository = GetNonNullService<ITunnelingRepository>();
        var command = new AddOrganizationCommand(_organizationId, _organizationName);
        var handler = new AddOrganizationHandler(repository);

        var result = await handler.Handle(command, cts.Token);

        result.ShouldNotBeNull();

        var organization = result.Data;
        organization.ShouldNotBeNull();
        organization.Id.ShouldBe(_organizationId);
        organization.Name.ShouldBe(_organizationName);   
    }

    [Fact, TestOrder(2)]
    [Trait("Category", "Integration")]    
    public async Task AddDeviceGroupCommandPassAsync()
    {
        var cts = new CancellationTokenSource();
        var repository = GetNonNullService<ITunnelingRepository>();
        var command = new AddDeviceGroupCommand(_deviceGroupId, _organizationId, _deviceGroupName, _serverLocation);
        var handler = new AddDeviceGroupHandler(GetLogger(), _fixture.Configuration, repository);

        var result = await handler.Handle(command, cts.Token);

        result.ShouldNotBeNull();

        var organization = result.Data;
        organization.ShouldNotBeNull();
        organization.Id.ShouldBe(_organizationId);
        organization.Name.ShouldBe(_organizationName);   
    }

    private static ILogger<AddDeviceGroupCommand> GetLogger() => Substitute.For<ILogger<AddDeviceGroupCommand>>();

    private T GetNonNullService<T>()
    {
        var service = _fixture.GetService<T>(_testOutputHelper);
        Assert.NotNull(service);
        return service;
    }
}