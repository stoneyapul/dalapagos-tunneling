namespace Dalapagos.Tunneling.Core.IntegrationTests;

using Commands;
using Infrastructure;
using Shouldly;
using Xunit.Abstractions;

public class DeleteDeviceGroupTests(ITestOutputHelper testOutputHelper, CoreTestFixture fixture)
    : DeviceGroupCoreTestBase(testOutputHelper, fixture)
{
    [Fact]
    [Trait("Category", "Integration")]    
    public async Task DeleteDeviceGroupCommandPassAsync()
    {
        var cts = new CancellationTokenSource();
        var repository = GetNonNullService<ITunnelingRepository>();
        var command = new DeleteDeviceGroupCommand(_deviceGroupId, _organizationId);
        var handler = new DeleteDeviceGroupHandler(GetLogger<DeleteDeviceGroupCommand>(), _fixture.Configuration!, repository);

        var result = await handler.Handle(command, cts.Token);

        result.ShouldNotBeNull();
        result.IsSuccessful.ShouldBeTrue();
    }
}