namespace Dalapagos.Tunneling.Core.Commands;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
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
    ITunnelingRepository tunnelingRepository) 
        : IRequestHandler<AddDeviceGroupCommand, OperationResult<DeviceGroup>>
{
    private const string BaseUrl = "https://dev.azure.com/dalapagos";
    private const string PipelineName = "dalapagos-tunneling-server-scripts";

    public async ValueTask<OperationResult<DeviceGroup>> Handle(AddDeviceGroupCommand request, CancellationToken cancellationToken)
    {
        var keyVaultName = config["KeyVaultName"]!;
        var projectId = new Guid(config["DevOpsProjectId"]!);
        var branch = config["DevOpsBranch"];
        var personalAccessToken = config["DevOpsPersonalAccessToken"]!;
        
        var deviceGroup = await tunnelingRepository.UpsertDeviceGroupAsync(
            request.Id.HasValue ? request.Id : Guid.NewGuid(), 
            request.OrganizationId,
            request.Name,
            request.Location,
            ServerStatus.Unknown,
            null, // TODO: security group
            null, // TODO: user group
            cancellationToken);

        var shortDeviceGrpId = deviceGroup.Id.ToString()!.Substring(24, 12).ToLowerInvariant();
        var resourceGroupName = $"dlpg-{shortDeviceGrpId}";
        var adminVmPasswordSecretName = $"{shortDeviceGrpId}-Tnls-VmPass";

        // Add VM password secret to key vault.
        logger.LogInformation("Saving VM password for organization {OrganizationId} device group {ShortDeviceGrpId}.", request.OrganizationId, shortDeviceGrpId);
        var credential = GetTokenCredential(config);
        var keyVaultUrl = "https://" + keyVaultName + ".vault.azure.net";
        var keyVaultClient = new SecretClient(new Uri(keyVaultUrl), credential);
        await keyVaultClient.SetSecretAsync(adminVmPasswordSecretName, CreateVmPassword(), cancellationToken);

        logger.LogInformation("Creating RPort server for organization {OrganizationId} device group {ShortDeviceGrpId}.", request.OrganizationId, shortDeviceGrpId);
 
        var pipelineClient = new PipelinesHttpClient(new Uri(BaseUrl), new VssBasicCredential(string.Empty, personalAccessToken));
        var pipelines = await pipelineClient.ListPipelinesAsync(projectId, cancellationToken: cancellationToken);
        var pipeline = pipelines.First(p => p.Name.Equals(PipelineName));

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
            deviceGroup.Id, 
            deviceGroup.OrganizationId,
            deviceGroup.Name,
            deviceGroup.ServerLocation,
            ServerStatus.Deploying,
            null, // TODO: security group
            null, // TODO: user group
            cancellationToken);

        return new OperationResult<DeviceGroup>(deviceGroup, true, []);
    }

    private static TokenCredential GetTokenCredential(IConfiguration config)
    {
        var adConfigSection = config.GetSection("AzureAd");
        var tenantId = adConfigSection.GetValue<string>("TenantId");
        var clientId = adConfigSection.GetValue<string>("ClientId");
        var clientSecret = adConfigSection.GetValue<string>("ClientSecret");
        return
            string.IsNullOrWhiteSpace(tenantId)
            || string.IsNullOrWhiteSpace(clientId)
            || string.IsNullOrWhiteSpace(clientSecret)
            ? new DefaultAzureCredential()
            : new ClientSecretCredential(tenantId, clientId, clientSecret);
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
