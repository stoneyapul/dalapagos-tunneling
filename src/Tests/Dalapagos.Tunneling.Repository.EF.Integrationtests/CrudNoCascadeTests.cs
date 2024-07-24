namespace Dalapagos.Tunneling.Repository.EF.Integrationtests;

using Core;
using Core.Infrastructure;
using Core.Model;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;
using Xunit.Microsoft.DependencyInjection.Attributes;

[TestCaseOrderer(
    "Xunit.Microsoft.DependencyInjection.TestsOrder.TestPriorityOrderer",
    "Xunit.Microsoft.DependencyInjection"
)]public class CrudNoCascadeTests(ITestOutputHelper testOutputHelper, RepositoryTestFixture fixture) 
    : TestBed<RepositoryTestFixture>(testOutputHelper, fixture)
{
    private readonly Guid _organizationId = new("2d80cca7-2591-43c5-a721-442a710d814a");
    private readonly string _organizationName1 = "Acme Rockets";
    private readonly string _organizationName2 = "Acme Rockets LLC.";
    private readonly Guid _deviceGroupId = new("4d80cca7-2591-43c5-a721-442a710d814b");
    private readonly string _deviceGroupName1 = "Eastern Region";
    private readonly string _deviceGroupName2 = "Florida Region";
    private readonly ServerLocation _serverLocation = ServerLocation.West;
    private readonly Guid _deviceId = new("5d80cca7-2591-43c5-a721-442a710d814c");
    private readonly string _deviceName1 = "Acme Controller 1";
    private readonly string _deviceName2 = "Acme Controller 2";


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
    public async Task UpdateOrganizationPassAsync()
    {
        var cts = new CancellationTokenSource();
        var tunnelingRepository = GetNonNullService<ITunnelingRepository>();

        var organization = await tunnelingRepository.UpsertOrganizationAsync(_organizationId, _organizationName2, cts.Token);
        organization.ShouldNotBeNull();
        organization.Id.ShouldBe(_organizationId);
        organization.Name.ShouldBe(_organizationName2);           
    }

    [Fact, TestOrder(3)]
    [Trait("Category", "Integration")]    
    public async Task AddDeviceGroupPassAsync()
    {
        var cts = new CancellationTokenSource();
        var tunnelingRepository = GetNonNullService<ITunnelingRepository>();

        var deviceGroup = await tunnelingRepository.UpsertDeviceGroupAsync(
            _deviceGroupId, 
            _organizationId, 
            _deviceGroupName1, 
            _serverLocation, 
            ServerStatus.Unknown, 
            Constants.FakeBaseUrl,
            cts.Token);

        deviceGroup.ShouldNotBeNull();
        deviceGroup.Id.ShouldBe(_deviceGroupId);
        deviceGroup.Name.ShouldBe(_deviceGroupName1);   
    }

    [Fact, TestOrder(4)]
    [Trait("Category", "Integration")]    
    public async Task UpsertDeviceGroupPassAsync()
    {
        var cts = new CancellationTokenSource();
        var tunnelingRepository = GetNonNullService<ITunnelingRepository>();

        var deviceGroup = await tunnelingRepository.UpsertDeviceGroupAsync(
            _deviceGroupId, 
            _organizationId, 
            _deviceGroupName2, 
            _serverLocation, 
            ServerStatus.Unknown, 
            Constants.FakeBaseUrl,
            cts.Token);

        deviceGroup.ShouldNotBeNull();
        deviceGroup.Id.ShouldBe(_deviceGroupId);
        deviceGroup.Name.ShouldBe(_deviceGroupName2);   
    }

    [Fact, TestOrder(5)]
    [Trait("Category", "Integration")]    
    public async Task AddDevicePassAsync()
    {
        var cts = new CancellationTokenSource();
        var tunnelingRepository = GetNonNullService<ITunnelingRepository>();

        var device = await tunnelingRepository.UpsertDeviceAsync(
            _deviceId,
            _deviceGroupId, 
            _deviceName1, 
            Os.Linux,
            cts.Token);

        device.ShouldNotBeNull();
        device.Id.ShouldBe(_deviceId);
        device.Name.ShouldBe(_deviceName1); 
        device.Os.ShouldBe(Os.Linux);  
    }

    [Fact, TestOrder(6)]
    [Trait("Category", "Integration")]    
    public async Task UpdateDevicePassAsync()
    {
        var cts = new CancellationTokenSource();
        var tunnelingRepository = GetNonNullService<ITunnelingRepository>();

        var device = await tunnelingRepository.UpsertDeviceAsync(
            _deviceId,
            _deviceGroupId, 
            _deviceName2, 
            Os.Windows,
            cts.Token);

        device.ShouldNotBeNull();
        device.Id.ShouldBe(_deviceId);
        device.Name.ShouldBe(_deviceName2);  
        device.Os.ShouldBe(Os.Windows); 
    }

    [Fact, TestOrder(7)]
    [Trait("Category", "Integration")]    
    public async Task DeleteDevicePassAsync()
    {
        var cts = new CancellationTokenSource();
        var tunnelingRepository = GetNonNullService<ITunnelingRepository>();

        await tunnelingRepository.DeleteDeviceAsync(_deviceId, cts.Token);
    }

    [Fact, TestOrder(8)]
    [Trait("Category", "Integration")]    
    public async Task DeleteDeviceGroupPassAsync()
    {
        var cts = new CancellationTokenSource();
        var tunnelingRepository = GetNonNullService<ITunnelingRepository>();

        await tunnelingRepository.DeleteDeviceGroupAsync(_deviceGroupId, cts.Token);
    }

    [Fact, TestOrder(9)]
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