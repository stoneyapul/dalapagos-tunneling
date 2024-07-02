namespace Dalapagos.Tunneling.Repository.EF.Integrationtests;

using Core.Infrastructure;
using Core.Model;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;
using Xunit.Microsoft.DependencyInjection.Attributes;

[TestCaseOrderer(
    "Xunit.Microsoft.DependencyInjection.TestsOrder.TestPriorityOrderer",
    "Xunit.Microsoft.DependencyInjection"
)]
public class DeviceCascadeTests(ITestOutputHelper testOutputHelper, RepositoryTestFixture fixture) 
    : TestBed<RepositoryTestFixture>(testOutputHelper, fixture)
{
    private readonly Guid _organizationId = new("ed80cca7-2591-43c5-a721-442a710d814b");
    private readonly string _organizationName = "Acme Rockets";
    private readonly Guid _deviceGroupId = new("ad80cca7-2591-43c5-a721-442a710d813b");
    private readonly string _deviceGroupName = "Western Region";
    private readonly ServerLocation _serverLocation = ServerLocation.West;
    private readonly Guid _deviceId = new("bd80cca7-2591-43c5-a721-442a710d813c");
    private readonly string _deviceName = "Acme Controller 1";
 

    [Fact, TestOrder(1)]
    [Trait("Category", "Integration")]    
    public async Task AddOrganizationPassAsync()
    {
        var cts = new CancellationTokenSource();
        var tunnelingRepository = GetNonNullService<ITunnelingRepository>();

        var organization = await tunnelingRepository.UpsertOrganizationAsync(_organizationId, _organizationName, cts.Token);
        organization.ShouldNotBeNull();
        organization.Id.ShouldBe(_organizationId);
        organization.Name.ShouldBe(_organizationName);   
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
            _deviceGroupName, 
            _serverLocation, 
            Core.Model.ServerStatus.Unknown, 
            cts.Token);

        deviceGroup.ShouldNotBeNull();
        deviceGroup.Id.ShouldBe(_deviceGroupId);
        deviceGroup.Name.ShouldBe(_deviceGroupName);   
    }

    [Fact, TestOrder(3)]
    [Trait("Category", "Integration")]    
    public async Task AddDeviceWithNoDeviceGroupPassAsync()
    {
        var cts = new CancellationTokenSource();
        var tunnelingRepository = GetNonNullService<ITunnelingRepository>();

        var device = await tunnelingRepository.UpsertDeviceAsync(
            _deviceId,
            null, 
            _deviceName, 
            Os.Linux,
            cts.Token);

        device.ShouldNotBeNull();
        device.Id.ShouldBe(_deviceId);
        device.Name.ShouldBe(_deviceName); 
        device.Os.ShouldBe(Os.Linux);  
    }

    [Fact, TestOrder(4)]
    [Trait("Category", "Integration")]    
    public async Task UpdateDeviceWithDeviceGroupPassAsync()
    {
        var cts = new CancellationTokenSource();
        var tunnelingRepository = GetNonNullService<ITunnelingRepository>();

        var device = await tunnelingRepository.UpsertDeviceAsync(
            _deviceId,
            _deviceGroupId, 
            _deviceName,
            Os.Windows, 
            cts.Token);

        device.ShouldNotBeNull();
        device.Id.ShouldBe(_deviceId);
        device.Name.ShouldBe(_deviceName);  
        device.Os.ShouldBe(Os.Windows); 
    }

    [Fact, TestOrder(5)]
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