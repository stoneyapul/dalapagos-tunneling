namespace Dalapagos.Tunneling.Core.IntegrationTests;

using Commands;
using Infrastructure;
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
    private readonly Guid _organizationId = new("de80cca7-2591-43c5-a721-442a710d814b");
    private readonly string _organizationName = "Acme Rockets";

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

   private T GetNonNullService<T>()
    {
        var service = _fixture.GetService<T>(_testOutputHelper);
        Assert.NotNull(service);
        return service;
    }
}