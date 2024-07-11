namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Exceptions;
using Infrastructure;
using Mediator;
using Microsoft.Extensions.Configuration;   
using Model;

public abstract class HandlerBase<TRequest, TResponse>(ITunnelingRepository tunnelingRepository) : IRequestHandler<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>, ICommandIdentity
    where TResponse : OperationResult
{
    protected readonly ITunnelingRepository tunnelingRepository = tunnelingRepository;

    protected static TokenCredential GetTokenCredential(IConfiguration config)
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

    public abstract ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken);

    protected async Task<IList<OrganizationUser>> GetUserOrganizationsAsync(TRequest request, CancellationToken cancellationToken)
    {
        return await tunnelingRepository.GetOrganizationUsersByUserIdAsync(request.UserId, cancellationToken);              
    }

    protected async Task VerifyUserOrganizationAsync(TRequest request, CancellationToken cancellationToken)
    {
        var userOrganizations = await GetUserOrganizationsAsync(request, cancellationToken);
        if (userOrganizations.All(x => x.OrganizationId != request.OrganizationId))
        {
            throw new AccessDeniedException();
        }
    }
}
