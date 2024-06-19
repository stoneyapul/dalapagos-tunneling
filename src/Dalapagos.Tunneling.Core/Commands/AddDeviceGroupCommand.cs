namespace Dalapagos.Tunneling.Core.Commands;

using System.Threading;
using System.Threading.Tasks;
using Dalapagos.Tunneling.Core.Extensions;
using Infrastructure;
using Mediator;
using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.Common;
using Model;

public record AddDeviceGroupCommand(Guid? Id, Guid OrganizationId, string Name, ServerLocation Location) : IRequest<OperationResult<DeviceGroup>>;

public class AddDeviceGroupHandler(
    ILogger<CreateServerHandler> logger, 
    IConfiguration config, 
    ITunnelingRepository tunnelingRepository) 
        : IRequestHandler<AddDeviceGroupCommand, OperationResult<DeviceGroup>>
{
    private const string BaseUrl = "https://dev.azure.com/dalapagos";
    private const string PipelineName = "server-scripts";

    public async ValueTask<OperationResult<DeviceGroup>> Handle(AddDeviceGroupCommand request, CancellationToken cancellationToken)
    {
        var shortOrgId = request.OrganizationId.ToString();
        shortOrgId = shortOrgId.Substring(shortOrgId.Length - 12, 12).ToLowerInvariant();
        var keyVaultName = config["KeyVaultName"]!;
        var resourceGroupName = $"dlpg-{shortOrgId}";
        var projectId = new Guid(config["DevOpsProjectId"]!);
        var branch = config["DevOpsBranch"];
        var personalAccessToken = config["DevOpsPersonalAccessToken"]!;

        var deviceGroup = await tunnelingRepository.UpsertDeviceGroupAsync(
            request.Id.HasValue ? request.Id : Guid.NewGuid(), 
            request.OrganizationId,
            request.Name,
            "tst",
            request.Location,
            ServerStatus.Unknown,
            null, //TODO
            null, //TODO
            cancellationToken);
            
        logger.LogInformation("Creating RPort server for {Organization}.", request.OrganizationId);

        var pipelineClient = new PipelinesHttpClient(new Uri(BaseUrl), new VssBasicCredential(string.Empty, personalAccessToken));
        var pipelines = await pipelineClient.ListPipelinesAsync(projectId, cancellationToken: cancellationToken);
        var pipeline = pipelines.First(p => p.Name.Equals(PipelineName));

        var pipelineParameters = new RunPipelineParameters
        {       
            TemplateParameters = new Dictionary<string, string>
            {
                { "orgId", shortOrgId },
                { "location", request.Location.ToAzureLocation() },
                { "resourceGroupName", resourceGroupName },
                { "keyVaultName", keyVaultName },
                { "adminVmPasswordSecretName", $"{shortOrgId}-Tnls-VmPass" }
            }
        };

        return new OperationResult<DeviceGroup>(deviceGroup, true, []);
    }
}
