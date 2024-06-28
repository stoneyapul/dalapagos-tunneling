namespace Dalapagos.Tunneling.Monitor.HF;

using Core.Infrastructure;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class HfMonitorInstaller
{
    public static void AddHfMonitor(this IServiceCollection services, IConfiguration? config)
    {
        ArgumentNullException.ThrowIfNull(config, nameof(config));

        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(config.GetConnectionString("TunnelsDb"), 
            new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            }));

        services.AddHangfireServer();
        services.AddScoped<IDeviceGroupDeploymentMonitor, HfDeviceGroupDeploymentMonitor>();
    }
}