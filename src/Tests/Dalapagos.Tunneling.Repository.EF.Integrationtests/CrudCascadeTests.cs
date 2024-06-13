namespace Dalapagos.Tunneling.Repository.EF.Integrationtests;

using Core.Infrastructure;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;
using Xunit.Microsoft.DependencyInjection.Attributes;

[TestCaseOrderer(
    "Xunit.Microsoft.DependencyInjection.TestsOrder.TestPriorityOrderer",
    "Xunit.Microsoft.DependencyInjection"
)]public class CrudCascadeTests(ITestOutputHelper testOutputHelper, RepositoryTestFixture fixture) 
    : TestBed<RepositoryTestFixture>(testOutputHelper, fixture)
{
    private readonly Guid _organizationId = new("1d80cca7-2591-43c5-a721-442a710d814b");
    private readonly string _organizationName1 = "Acme Rockets";
    private readonly Guid _deviceGroupId = new("ad80cca7-2591-43c5-a721-442a710d814b");
    private readonly string _deviceGroupName1 = "Eastern Region";
    private readonly string _serverName = "Server 1";
    private readonly string _serverLocation = "EastUS";
    private readonly Guid _deviceId = new("dd80cca7-2591-43c5-a721-442a710d814c");
    private readonly string _deviceName1 = "Acme Controller 1";
 

    [Fact, TestOrder(1)]
    [Trait("Category", "Integration")]    
    public async Task AddOrganizationPassAsync()
    {
        var cts = new CancellationTokenSource();
        var tunnelingRepository = GetNonNullService<ITunnelingRepository>();

        var organization = await tunnelingRepository.UpsertOrganizationAsync(_organizationId, _organizationName1, cts.Token);
        organization.ShouldNotBeNull();
        organization.Id.ShouldBe(_organizationId);
        organization.Name.ShouldBe(_organizationName1);   
    }

    [Fact, TestOrder(2)]
    [Trait("Category", "Integration")]    
    public async Task AddDeviceGroupPassAsync()
    {
        var cts = new CancellationTokenSource();
        var tunnelingRepository = GetNonNullService<ITunnelingRepository>();

        var deviceGroup = await tunnelingRepository.UpsertDeviceGroupAsync(
            _deviceGroupId, 
            _organizationId, 
            _deviceGroupName1, 
            _serverName, 
            _serverLocation, 
            Core.Model.ServerStatus.Unknown, 
            null, 
            null, 
            cts.Token);

        deviceGroup.ShouldNotBeNull();
        deviceGroup.Id.ShouldBe(_deviceGroupId);
        deviceGroup.Name.ShouldBe(_deviceGroupName1);   
    }

    [Fact, TestOrder(3)]
    [Trait("Category", "Integration")]    
    public async Task AddDevicePassAsync()
    {
        var cts = new CancellationTokenSource();
        var tunnelingRepository = GetNonNullService<ITunnelingRepository>();

        var device = await tunnelingRepository.UpsertDeviceAsync(
            _deviceId,
            _deviceGroupId, 
            _deviceName1, 
            cts.Token);

        device.ShouldNotBeNull();
        device.Id.ShouldBe(_deviceId);
        device.Name.ShouldBe(_deviceName1);   
    }

    [Fact, TestOrder(4)]
    [Trait("Category", "Integration")]    
    public async Task DeleteOrganizationPassAsync()
    {
        var cts = new CancellationTokenSource();
        var tunnelingRepository = GetNonNullService<ITunnelingRepository>();

        await tunnelingRepository.DeleteOrganizationAsync(_organizationId, cts.Token);
    }

   private T GetNonNullService<T>()
    {
        var service = _fixture.GetService<T>(_testOutputHelper);
        Assert.NotNull(service);
        return service;
    }
}