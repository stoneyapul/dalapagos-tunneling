// TODO: Review whether injecting another infrastructure service into this infrastructure service is a good idea..
namespace Dalapagos.Tunneling.Monitor.HF;

using System;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Infrastructure;
using Hangfire;
using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.VisualStudio.Services.Common;

public class HfDeviceGroupDeploymentMonitor(ITunnelingRepository tunnelingRepository) : IDeviceGroupDeploymentMonitor
{
    public async Task MonitorAsync(Guid deviceGroupId, Guid projectId, int pipelineId, int runId, string personalAccessToken, CancellationToken cancellationToken)
    {
        BackgroundJob.Schedule(
            () =>
            WatchAndUpdateStatusAsync(deviceGroupId, projectId, pipelineId, runId, personalAccessToken, cancellationToken),
            TimeSpan.FromSeconds(90));
    }

    private async Task WatchAndUpdateStatusAsync(Guid deviceGroupId, Guid projectId, int pipelineId, int runId, string personalAccessToken, CancellationToken cancellationToken)
    {
        var pipelineClient = new PipelinesHttpClient(new Uri(Constants.DevOpsBaseUrl), new VssBasicCredential(string.Empty, personalAccessToken));

        while (!cancellationToken.IsCancellationRequested)
        {
            // TODO: Finish this!
            var pipelineRun = await pipelineClient.GetRunAsync(projectId, pipelineId, runId, cancellationToken: cancellationToken);
            await tunnelingRepository.UpdateDeviceGroupServerStatusAsync(deviceGroupId, Core.Model.ServerStatus.Deployed, cancellationToken);
            break;
        }  
    }
}
