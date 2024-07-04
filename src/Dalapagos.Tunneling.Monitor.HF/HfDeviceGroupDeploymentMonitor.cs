// TODO: Review whether injecting another infrastructure service into this infrastructure service is a good idea..
namespace Dalapagos.Tunneling.Monitor.HF;

using System;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Infrastructure;
using Core.Model;
using Hangfire;
using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.Common;

public class HfDeviceGroupDeploymentMonitor(ITunnelingRepository tunnelingRepository, ILogger<HfDeviceGroupDeploymentMonitor> logger) : IDeviceGroupDeploymentMonitor
{
    public void MonitorDeployment(Guid deviceGroupId, Guid projectId, int pipelineId, int runId, string personalAccessToken, CancellationToken cancellationToken)
    {
        logger.LogInformation("Monitoring deployment for device group {DeviceGroupId} pipeline {PipelineId} run {RunId}.", deviceGroupId, pipelineId, runId);

        BackgroundJob.Schedule(
            () =>
            WatchAndUpdateStatusAsync(deviceGroupId, projectId, pipelineId, runId, personalAccessToken, cancellationToken),
            TimeSpan.FromSeconds(60));
    }

    public async Task WatchAndUpdateStatusAsync(Guid deviceGroupId, Guid projectId, int pipelineId, int runId, string personalAccessToken, CancellationToken cancellationToken)
    {
        var pipelineClient = new PipelinesHttpClient(new Uri(Constants.DevOpsBaseUrl), new VssBasicCredential(string.Empty, personalAccessToken));
 
        while (!cancellationToken.IsCancellationRequested)
        {
            var pipelineRun = await pipelineClient.GetRunAsync(projectId, pipelineId, runId, cancellationToken: cancellationToken);

            logger.LogInformation("Deployment for device group {DeviceGroupId} pipeline {PipelineId} run {RunId} has state {State}.", deviceGroupId, pipelineId, runId, pipelineRun.State);
            if (pipelineRun.Result.HasValue)
            {
                logger.LogInformation("Deployment for device group {DeviceGroupId} pipeline {PipelineId} run {RunId} has result {Result}.", deviceGroupId, pipelineId, runId, pipelineRun.Result);
            }
            
            ServerStatus status = pipelineRun.State switch
            {
                RunState.Completed => ServerStatus.Deployed,
                RunState.InProgress => ServerStatus.Deploying,
                RunState.Canceling => ServerStatus.DeployFailed,
                _ => ServerStatus.Error,
            };

            await tunnelingRepository.UpdateDeviceGroupServerStatusAsync(deviceGroupId, status, cancellationToken);

            if (pipelineRun.State != RunState.InProgress)
            {
                if (pipelineRun.Result.HasValue && pipelineRun.Result.Value.Equals("Succeeded"))
                {
                    await tunnelingRepository.UpdateDeviceGroupServerStatusAsync(deviceGroupId, ServerStatus.Online, cancellationToken);
                }

                break;
            }

            await Task.Delay(20000, cancellationToken);
        }  
    }
}
