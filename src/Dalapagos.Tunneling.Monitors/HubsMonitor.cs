namespace Dalapagos.Tunneling.Monitors;

using Core.Infrastructure;
using Core.Model;

public class HubsMonitor(IServiceScopeFactory serviceScopeFactory, ILogger<HubsMonitor> logger) : BackgroundService
{
    private const int DelayMs = 5000;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Start monitoring hubs.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                var tunnelingProvider = scope.ServiceProvider.GetRequiredService<ITunnelingProvider>();
                var tunnelingRepository = scope.ServiceProvider.GetRequiredService<ITunnelingRepository>();

                var organizationSummaries = await tunnelingRepository.GetOrganizationsAsync(stoppingToken);               
                if (organizationSummaries == null)
                {
                    await Task.Delay(DelayMs, stoppingToken);
                    continue;
                }

                foreach (var organizationSummary in organizationSummaries)
                {
                    if (!organizationSummary.Id.HasValue)
                    {
                        continue;
                    }

                    var organization = await tunnelingRepository.RetrieveOrganizationAsync(organizationSummary.Id.Value, stoppingToken);
                    if (organization == null || organization.DeviceGroups == null)
                    {
                        continue;
                    }

                    foreach (var deviceGroupSummary in organization.DeviceGroups)
                    {
                        if (!deviceGroupSummary.Id.HasValue)
                        {
                            continue;
                        }

                        var deviceGroup = await tunnelingRepository.RetrieveDeviceGroupAsync(
                            organizationSummary.Id.Value, 
                            deviceGroupSummary.Id.Value, 
                            stoppingToken);

                        // Check if the device group is online or in error state. Any other status means that the device group is still being deployed.
                        if (deviceGroup.ServerStatus != ServerStatus.Online && deviceGroup.ServerStatus != ServerStatus.Error)
                        {
                            continue;
                        }   

                        if (string.IsNullOrWhiteSpace(deviceGroup.ServerBaseUrl))
                        {
                            logger.LogWarning("Device group {DeviceGroupName} does not have a server base URL.", deviceGroupSummary.Name);
                            continue;
                        }

                        logger.LogInformation("Checking status of {ServerName}.", deviceGroupSummary.Name);

                        // Check the tunneling server.
                        var server = await tunnelingProvider.GetServerInformationAsync(
                            deviceGroupSummary.Id.Value,
                            deviceGroup.ServerBaseUrl, 
                            stoppingToken);
                    }                  
                }
           }

            await Task.Delay(DelayMs, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Stop monitoring hubs.");
        await base.StopAsync(stoppingToken);
    }
}
