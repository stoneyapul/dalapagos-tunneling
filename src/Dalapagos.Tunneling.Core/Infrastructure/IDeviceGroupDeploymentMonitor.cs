namespace Dalapagos.Tunneling.Core.Infrastructure;

public interface IDeviceGroupDeploymentMonitor
{
    void MonitorDeployment(Guid deviceGroupId, Guid projectId, int pipelineId, int runId, string personalAccessToken, CancellationToken cancellationToken);
}
