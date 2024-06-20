namespace Dalapagos.Tunneling.Api.Endpoints;

using Core.Commands;
using Mediator;
using Security;

public static class Servers
{
    public static void RegisterServerEndpoints(this IEndpointRouteBuilder routes)
    {
        var endpoints = routes.MapGroup("v1/{orgId}/servers")
            .RequireAuthorization(Policies.RportAdminPolicy);

        endpoints.MapPost("", async (string orgId, CreateServerRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            //var result = await mediator.Send(new CreateServerCommand(orgId, request.Location), cancellationToken);
            //return TypedResults.Accepted(result.Data, result);
        })
        .WithName("CreateServer");
    }
}