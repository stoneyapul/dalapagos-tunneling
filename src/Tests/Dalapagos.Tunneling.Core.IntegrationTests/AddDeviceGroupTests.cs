namespace Dalapagos.Tunneling.Core.IntegrationTests;

using Commands;
using Infrastructure;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Attributes;

[TestCaseOrderer(
    "Xunit.Microsoft.DependencyInjection.TestsOrder.TestPriorityOrderer",
    "Xunit.Microsoft.DependencyInjection"
)]
public class AddDeviceGroupTests(ITestOutputHelper testOutputHelper, CoreTestFixture fixture)
    : DeviceGroupCoreTestBase(testOutputHelper, fixture)
{
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
        var handler = new AddDeviceGroupHandler(GetLogger<AddDeviceGroupCommand>(), _fixture.Configuration!, repository);

        var result = await handler.Handle(command, cts.Token);

        result.ShouldNotBeNull();
        result.IsSuccessful.ShouldBeTrue();
 
        var deviceGroup = result.Data;
        deviceGroup.ShouldNotBeNull();
        deviceGroup.Id.ShouldBe(_deviceGroupId);
        deviceGroup.Name.ShouldBe(_deviceGroupName);   
    }
}