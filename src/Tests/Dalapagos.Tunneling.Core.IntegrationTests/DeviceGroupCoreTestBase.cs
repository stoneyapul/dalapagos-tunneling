namespace Dalapagos.Tunneling.Core.IntegrationTests;

using Model;
using Xunit.Abstractions;

public abstract class DeviceGroupCoreTestBase(ITestOutputHelper testOutputHelper, CoreTestFixture fixture)
    : CoreTestBase(testOutputHelper, fixture)
{
    protected readonly Guid _organizationId = new("ce80cca7-2591-43c5-a721-442a610d814b");
    protected readonly string _organizationName = "Acme Rockets";
    protected readonly Guid _deviceGroupId = new("cd80cca7-2591-43c5-a721-442a710d812b");
    protected readonly string _deviceGroupName = "Western Region";
    protected readonly ServerLocation _serverLocation = ServerLocation.West;
}