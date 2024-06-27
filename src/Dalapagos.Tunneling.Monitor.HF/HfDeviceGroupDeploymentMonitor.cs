namespace Dalapagos.Tunneling.Monitor.HF;

using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Infrastructure;

public class HfDeviceGroupDeploymentMonitor : IDeviceGroupDeploymentMonitor
{
    public Task MonitorAsync(Guid deviceGroupId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
