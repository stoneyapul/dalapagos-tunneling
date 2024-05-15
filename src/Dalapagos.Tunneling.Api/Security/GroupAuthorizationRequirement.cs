namespace Dalapagos.Tunneling.Api.Security;

using Microsoft.AspNetCore.Authorization;

public class GroupAuthorizationRequirement(string[] groupIds) : IAuthorizationRequirement
{
    public string[] GroupIds { get; init; } = groupIds;
}