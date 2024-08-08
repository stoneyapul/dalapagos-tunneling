namespace Dalapagos.Tunneling.Core.Handlers;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Exceptions;
using Infrastructure;
using Mediator;
using Microsoft.Extensions.Configuration;   
using Model;

public abstract class HandlerBase<TRequest, TResponse>(ITunnelingRepository tunnelingRepository, IConfiguration config) : IRequestHandler<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>, ICommandIdentity
    where TResponse : OperationResult
{
    private const string AzureAdKey = "AzureAd";
    private const string TunnelingAdminKey = "Groups:Dalapagos-Tunneling-Admin-Access";
    private const string TunnelingUserKey = "Groups:Dalapagos-Tunneling-User-Access";

    protected readonly ITunnelingRepository tunnelingRepository = tunnelingRepository;
    protected readonly IConfiguration config = config;

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

    protected static string CreatePassword()
    {
        const string specialChars = "!@#$?";
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
    
        for (var i = 0; i < 17; i++)
        {
            password.Append(allChars[random.Next(allChars.Length)]);
        }

        return password.ToString();
    }

    public abstract ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken);

    protected async Task<IList<OrganizationUser>> GetUserOrganizationsAsync(TRequest request, CancellationToken cancellationToken)
    {
        return await tunnelingRepository.GetOrganizationUsersByUserIdAsync(request.UserId, cancellationToken);              
    }

    protected async Task<AccessType> VerifyUserOrganizationAsync(TRequest request, CancellationToken cancellationToken)
    {
        var adConfigSection = config.GetSection(AzureAdKey) ?? throw new ConfigurationException(AzureAdKey);
        var adminGroup = adConfigSection.GetValue<string>(TunnelingAdminKey) ?? throw new ConfigurationException(TunnelingAdminKey);
        var userGroup = adConfigSection.GetValue<string>(TunnelingUserKey) ?? throw new ConfigurationException(TunnelingUserKey);

        var userOrganizations = await GetUserOrganizationsAsync(request, cancellationToken);
        if (userOrganizations.All(x => x.OrganizationId != request.OrganizationId))
        {
            throw new AccessDeniedException();
        }

        var group =  userOrganizations.First(x => x.OrganizationId == request.OrganizationId);
        return group.SecurityGroupId.Equals(adminGroup) ? AccessType.Admin : AccessType.User;
    }
}
