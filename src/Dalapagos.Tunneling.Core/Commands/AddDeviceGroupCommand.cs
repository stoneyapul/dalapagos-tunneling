﻿// Device groups are a collection of devices. Each device group has an RPort tunneling server associated with it. 
// This command creates a device group in the database and kicks off provisioning for an RPort server.
namespace Dalapagos.Tunneling.Core.Commands;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;
using Exceptions;
using Extensions;
using Infrastructure;
using Mediator;
using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.Common;
using Model;

public record AddDeviceGroupCommand(Guid? Id, Guid OrganizationId, string Name, ServerLocation Location) : IRequest<OperationResult<DeviceGroup>>;

public class AddDeviceGroupHandler(
    ILogger<AddDeviceGroupCommand> logger, 
    IConfiguration config, 
    ITunnelingRepository tunnelingRepository,
    IDeviceGroupDeploymentMonitor deploymentMonitor) 
        : CommandBase, IRequestHandler<AddDeviceGroupCommand, OperationResult<DeviceGroup>>
{

    public async ValueTask<OperationResult<DeviceGroup>> Handle(AddDeviceGroupCommand request, CancellationToken cancellationToken)
    {
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

        var deviceGroup = await tunnelingRepository.UpsertDeviceGroupAsync(
            request.Id.HasValue ? request.Id : Guid.NewGuid(), 
            request.OrganizationId,
            request.Name,
            request.Location,
            ServerStatus.Unknown,
            cancellationToken);

        var deviceGroupId = deviceGroup.Id ?? throw new Exception("Device group id is null.");
        var shortDeviceGrpId = deviceGroupId.ToString().Substring(24, 12).ToLowerInvariant();
        var resourceGroupName = $"dlpg-{shortDeviceGrpId}";
        var adminVmPasswordSecretName = $"{shortDeviceGrpId}-Tnls-VmPass";

        // Add VM password secret to key vault.
        logger.LogInformation("Saving VM password for organization {OrganizationId} device group {ShortDeviceGrpId}.", request.OrganizationId, shortDeviceGrpId);
        var credential = GetTokenCredential(config);
        var keyVaultUrl = "https://" + keyVaultName + ".vault.azure.net";
        var keyVaultClient = new SecretClient(new Uri(keyVaultUrl), credential);
        await keyVaultClient.SetSecretAsync(adminVmPasswordSecretName, CreateVmPassword(), cancellationToken);

        logger.LogInformation("Creating RPort server for organization {OrganizationId} device group {ShortDeviceGrpId}.", request.OrganizationId, shortDeviceGrpId);
 
        var projectId = new Guid(projectIdAsString);
        var pipelineClient = new PipelinesHttpClient(new Uri(Constants.DevOpsBaseUrl), new VssBasicCredential(string.Empty, personalAccessToken));
        var pipelines = await pipelineClient.ListPipelinesAsync(projectId, cancellationToken: cancellationToken);
        var pipeline = pipelines.First(p => p.Name.Equals(Constants.PipelineName));

        var pipelineParameters = new RunPipelineParameters
        {       
            TemplateParameters = new Dictionary<string, string>
            {
                { "deviceGrpId", shortDeviceGrpId },
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
            deviceGroupId, 
            deviceGroup.OrganizationId,
            deviceGroup.Name,
            deviceGroup.ServerLocation,
            ServerStatus.Deploying,
            cancellationToken);

        // Monitor the deploment pipeline.
        await deploymentMonitor.MonitorAsync(deviceGroupId, projectId, pipelineRun.Pipeline.Id, pipelineRun.Id, personalAccessToken, cancellationToken);
        
        return new OperationResult<DeviceGroup>(deviceGroup, true, Constants.StatusSuccessCreated, []);
    }
        
    private static string CreateVmPassword()
    {
        const string specialChars = "!@#$;:?";
        const string numbers = "0123456789";
        const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        const string allChars = specialChars + numbers + upperCase + lowerCase;

        var password = new StringBuilder();
        var random = new Random();

        password.Append(lowerCase[random.Next(lowerCase.Length)]);
        password.Append(specialChars[random.Next(specialChars.Length)]);
        password.Append(upperCase[random.Next(upperCase.Length)]);
        password.Append(numbers[random.Next(numbers.Length)]);
    
        for (var i = 0; i < 8; i++)
        {
            password.Append(allChars[random.Next(allChars.Length)]);
        }

        return password.ToString();
    }
}
