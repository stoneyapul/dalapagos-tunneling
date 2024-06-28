namespace Dalapagos.Tunneling.Core.Infrastructure;

public interface IDeviceGroupDeploymentMonitor
{
    Task MonitorAsync(Guid deviceGroupId, Guid projectId, int pipelineId, int runId, string personalAccessToken, CancellationToken cancellationToken);
}
