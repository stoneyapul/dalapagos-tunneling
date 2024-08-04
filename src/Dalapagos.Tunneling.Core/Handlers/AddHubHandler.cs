// Hubs are a collection of devices, a tunneling server, and a device group record in the database. 
// This command creates a device group in the database and kicks off provisioning for a tunneling server.
namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Commands;
using Exceptions;
using Extensions;
using Infrastructure;
using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.Common;
using Model;

internal sealed class AddHubHandler(
    ILogger<AddHubCommand> logger, 
    IConfiguration config, 
    ISecrets secrets,
    ITunnelingRepository tunnelingRepository,
    IDeviceGroupDeploymentMonitor deploymentMonitor) 
        : HandlerBase<AddHubCommand, OperationResult<Hub>>(tunnelingRepository, config)
{
    public override async ValueTask<OperationResult<Hub>> Handle(AddHubCommand request, CancellationToken cancellationToken)
    {
        _ = await VerifyUserOrganizationAsync(request, cancellationToken);
 
        var keyVaultName = config["KeyVaultName"] ?? throw new ConfigurationException("KeyVaultName");
        var projectIdAsString = config["DevOpsProjectId"] ?? throw new ConfigurationException("DevOpsProjectId");
        var branch = config["DevOpsBranch"] ?? throw new ConfigurationException("DevOpsBranch");
        var personalAccessToken = config["DevOpsPersonalAccessToken"] ?? throw new ConfigurationException("DevOpsPersonalAccessToken");
        
        // See if a request with the same Name and Location is already running.
        var organization = await tunnelingRepository.RetrieveOrganizationAsync(request.OrganizationId, cancellationToken);
        if (organization.DeviceGroups != null 
            && organization.DeviceGroups.Any(dg => 
                dg.ServerLocation == request.Location 
                && dg.Name.Equals(request.Name) 
                && dg.ServerStatus != ServerStatus.Online 
                && dg.ServerStatus != ServerStatus.Deployed))
        {
            throw new Exception($"Device group {request.Name} is already deploying to {request.Location}.");
        }

        var hubId = request.Id ?? Guid.NewGuid();
        var shorthubId = hubId.ToShortHubId();
        var resourceGroupName = $"dlpg-{shorthubId}";
        var adminVmPasswordSecretName = $"{shorthubId}{Constants.TunnelingServerVmPassNameSfx}";
        var fdqn = $"dalapagos-{shorthubId}.{request.Location.ToAzureLocation()}.cloudapp.azure.com";

        var deviceGroup = await tunnelingRepository.UpsertDeviceGroupAsync(
            hubId, 
            request.OrganizationId,
            request.Name,
            request.Location,
            ServerStatus.Unknown,
            fdqn,
            cancellationToken);

        // Add VM password secret to key vault.
        logger.LogInformation("Saving VM password for organization {OrganizationId} device group {ShortHubId}.", request.OrganizationId, shorthubId);
        await secrets.SetSecretAsync(adminVmPasswordSecretName, CreatePassword(), cancellationToken);
 
        logger.LogInformation("Creating tunneling server for organization {OrganizationId} device group {ShortHubId}.", request.OrganizationId, shorthubId);
        var projectId = new Guid(projectIdAsString);
        var pipelineClient = new PipelinesHttpClient(new Uri(Constants.DevOpsBaseUrl), new VssBasicCredential(string.Empty, personalAccessToken));
        var pipelines = await pipelineClient.ListPipelinesAsync(projectId, cancellationToken: cancellationToken);
        var pipeline = pipelines.First(p => p.Name.Equals(Constants.PipelineName));

        var pipelineParameters = new RunPipelineParameters
        {       
            TemplateParameters = new Dictionary<string, string>
            {
                { "hubId", shorthubId },
                { "location", request.Location.ToAzureLocation() },
                { "resourceGroupName", resourceGroupName },
                { "keyVaultName", keyVaultName },
                { "adminVmPasswordSecretName", adminVmPasswordSecretName }
            }
        };

       if (!string.IsNullOrWhiteSpace(branch))
        {
            var resources = new RunResourcesParameters();
            resources.Repositories.Add("self", new RepositoryResourceParameters { RefName = branch});
            pipelineParameters.Resources = resources;
        }

        var pipelineRun = await pipelineClient.RunPipelineAsync(pipelineParameters, projectId, pipeline.Id, cancellationToken: cancellationToken);

        deviceGroup = await tunnelingRepository.UpsertDeviceGroupAsync(
            hubId, 
            deviceGroup.OrganizationId,
            deviceGroup.Name,
            deviceGroup.ServerLocation,
            ServerStatus.Deploying,
            fdqn,
            cancellationToken);

        // Monitor the deploment pipeline.
        deploymentMonitor.MonitorDeployment(hubId, projectId, pipelineRun.Pipeline.Id, pipelineRun.Id, personalAccessToken, cancellationToken);
        
        return new OperationResult<Hub>(new Hub(deviceGroup), true, Constants.StatusSuccessAccepted, []);
    }
}
