namespace Dalapagos.Tunneling.Api.Security;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Graph;

public class GroupAuthorizationHandler(GraphServiceClient graphServiceClient) : AuthorizationHandler<GroupAuthorizationRequirement>
{
    private readonly GraphServiceClient graphServiceClient = graphServiceClient;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, GroupAuthorizationRequirement requirement)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            if (context.User.Claims.Any(c => c.Type.Equals("groups") && requirement.GroupIds.Any(r => r.Equals(c.Value))))
            {
                context.Succeed(requirement);
                return;
            }

            if (context.User.Claims.Any(c => c.Type.Equals("hasgroups") && c.Value.Equals("true")))
            {
                var groupsResult = await graphServiceClient.Me
                    .CheckMemberGroups(requirement.GroupIds)
                    .Request()
                    .PostAsync();

                foreach (var groupId in requirement.GroupIds)
                {
                    if (groupsResult.Any(g => g.Equals(groupId)))
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }
            }
        }
    }
}