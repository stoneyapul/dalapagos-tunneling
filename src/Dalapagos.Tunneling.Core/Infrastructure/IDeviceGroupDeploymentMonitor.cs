namespace Dalapagos.Tunneling.Core.Infrastructure;

public interface IDeviceGroupDeploymentMonitor
{
    Task MonitorAsync(Guid deviceGroupId, CancellationToken cancellationToken);
}
