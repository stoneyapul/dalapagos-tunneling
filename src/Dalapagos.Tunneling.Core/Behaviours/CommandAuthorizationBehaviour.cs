namespace Dalapagos.Tunneling.Core.Behaviours;

using System.Reflection;
using Commands;
using Exceptions;
using Infrastructure;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;

public sealed class CommandAuthorizationBehaviour<TMessage, TResponse>(
    ILogger<CommandAuthorizationBehaviour<TMessage, TResponse>> logger, 
    IConfiguration config, 
    ITunnelingRepository tunnelingRepository) 
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : CommandBase<TResponse>, IMessage
    where TResponse : OperationResult
{
    private const string AzureAdKey = "AzureAd";
    private const string TunnelingAdminKey = "Groups:Dalapagos-Tunneling-Admin-Access";
    private const string TunnelingUserKey = "Groups:Dalapagos-Tunneling-User-Access";
    private const string AccessDeniedMessage = "Access denied on {Command}.";

    public async ValueTask<TResponse> Handle(TMessage message, CancellationToken cancellationToken, MessageHandlerDelegate<TMessage, TResponse> next)
    {
        var attribute = typeof(TMessage).GetCustomAttribute<CommandAuthorizationAttribute>();
        if (attribute == null)
        {
            // No attribute, no need to check for authorization.
            return await next(message, cancellationToken);
        }
 
        var adConfigSection = config.GetSection(AzureAdKey) ?? throw new ConfigurationException(AzureAdKey);
        var adminGroup = adConfigSection.GetValue<string>(TunnelingAdminKey) ?? throw new ConfigurationException(TunnelingAdminKey);
        var userGroup = adConfigSection.GetValue<string>(TunnelingUserKey) ?? throw new ConfigurationException(TunnelingUserKey);
        var groupAsString = attribute.AccessType == AccessType.Admin ? adminGroup : userGroup;

        if (!Guid.TryParse(groupAsString, out Guid group))
        {
            logger.LogError(AccessDeniedMessage, message.GetType());
            return CreateResponse();          
        }

        var organizationUsers = await tunnelingRepository.GetOrganizationUsersByUserIdAsync(message.UserId, cancellationToken);              
        if (!organizationUsers.Any(o => o.OrganizationId == message.OrganizationId && o.SecurityGroupId == group))
        {
            logger.LogError(AccessDeniedMessage, message.GetType());
            return CreateResponse();          
        }

        return await next(message, cancellationToken);
    }

    private static TResponse CreateResponse()
    {
            var responseType = typeof(TResponse);
            string[] errors = [ $"Access denied on {typeof(TMessage).Name}." ];
            
            return responseType.IsGenericType
                ? (Activator.CreateInstance(responseType, [default, false, Constants.StatusPermissionDenied, errors]) as TResponse)!
                : (Activator.CreateInstance(responseType, [false, Constants.StatusPermissionDenied, errors]) as TResponse)!;
    }
}
