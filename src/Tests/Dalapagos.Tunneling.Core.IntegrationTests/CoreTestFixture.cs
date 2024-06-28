namespace Dalapagos.Tunneling.Core.IntegrationTests;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository.EF;
using Monitor.HF;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

public class CoreTestFixture : TestBedFixture
{
    protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
    {
        services.AddEfTunnelingRepository(configuration);
        services.AddHfMonitor(configuration);
    }

    protected override ValueTask DisposeAsyncCore() => new();
    
    protected override IEnumerable<TestAppSettings> GetTestAppSettings()
    {
        yield return new() { Filename = "appsettings.json" };
    }
}
