namespace Dalapagos.Tunneling.Core.Commands;

using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.Common;
using Model;

public record CreateServerCommand(string OrgId, string Location) : IRequest<OperationResult<string>>;

public class CreateServerHandler(ILogger<CreateServerHandler> logger, IConfiguration config) : IRequestHandler<CreateServerCommand, OperationResult<string>>
{
    private const string BaseUrl = "https://dev.azure.com/dalapagos";
    private const string PipelineName = "deploy-rport-server";

    public async ValueTask<OperationResult<string>> Handle(CreateServerCommand request, CancellationToken cancellationToken)
    {
        var keyVaultName = config["KeyVaultName"]!;
        var resourceGroupName = $"Dalapagos-{request.OrgId}";
        var projectId = new Guid(config["DevOpsProjectId"]!);
        var branch = config["DevOpsBranch"];
        var personalAccessToken = config["DevOpsPersonalAccessToken"]!;

        logger.LogInformation("Creating RPort server for {Org}.", request.OrgId);

        var pipelineClient = new PipelinesHttpClient(new Uri(BaseUrl), new VssBasicCredential(string.Empty, personalAccessToken));
        var pipelines = await pipelineClient.ListPipelinesAsync(projectId, cancellationToken: cancellationToken);
        var pipeline = pipelines.First(p => p.Name.Equals(PipelineName));

        var pipelineParameters = new RunPipelineParameters
        {       
            TemplateParameters = new Dictionary<string, string>
            {
                { "orgId", request.OrgId },
                { "location", request.Location },
                { "resourceGroupName", resourceGroupName },
                { "keyVaultName", keyVaultName },
            }
        };

        if (!string.IsNullOrWhiteSpace(branch))
        {
            var resources = new RunResourcesParameters();
            resources.Repositories.Add("self", new RepositoryResourceParameters { RefName = branch});
            pipelineParameters.Resources = resources;
        }

        var pipelineRun = await pipelineClient.RunPipelineAsync(pipelineParameters, projectId, pipeline.Id, cancellationToken: cancellationToken);

        logger.LogInformation("RPort server created for {Org}.", request.OrgId);

        return new OperationResult<string>(pipelineRun.Url, true, []);
    }
}
