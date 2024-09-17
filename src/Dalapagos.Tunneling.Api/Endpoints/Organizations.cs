namespace Dalapagos.Tunneling.Api.Endpoints;

using Core.Queries;
using Extensions;
using Mappers;
using Mediator;
using Security;

public static class Organizations
{
    public static void RegisterOrganizationEndpoints(this IEndpointRouteBuilder routes)
    {
        var endpoints = routes.MapGroup("/v1/organizations")
            .WithName("Organizations");

        endpoints.MapGet("", async (IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new GetOrganizationsByUserIdQuery(
                    context.User.GetUserId()), 
                cancellationToken);

            var mapper = new OrganizationsMapper();
            return mapper.MapOperationResult(result);
        })
        .WithName("Get Organizations")
        .WithTags("Organizations")
        .WithOpenApi(op =>
        {
            op.Description = "Get a list of organizations.";
            return op;
        })
        .RequireAuthorization(SecurityPolicies.TunnelingUserPolicy)
        .SetResponseStatusCode();      
    }
}
