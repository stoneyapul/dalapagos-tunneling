namespace Dalapagos.Tunneling.Api.Endpoints;

using Core.Commands;
using Core.Model;
using Dto;
using Extensions;
using Mappers;
using Mediator;
using Security;
using Validation;

public static class Tunnels
{
    public static void RegisterTunnelEndpoints(this IEndpointRouteBuilder routes)
    {
        var endpoints = routes.MapGroup("/v1/organizations/{organizationId}/tunnels")
            .WithName("Tunnels");

        endpoints.MapPost("", async (Guid organizationId, AddTunnelRequest request, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new AddTunnelCommand(
                    request.DeviceId,
                    Enum.Parse<Protocol>(request.Protocol, true),
                    request.DeleteAfterMin,
                    request.Port,
                    request.AllowedIp,
                    organizationId,
                    context.User.GetUserId()),
                cancellationToken);

            var mapper = new DeviceMapper();
            return mapper.MapOperationResult(result);
        })
        .WithName("Create tunnel")
        .WithTags("Tunnels")
        .WithOpenApi(op =>
        {
            op.Description = "Create a tunnel to a device.";
            op.Parameters[0].Description = "A globally unique identifier that represents the organization.";
            return op;
        })
        .Validate<AddTunnelRequest>()
        .RequireAuthorization(SecurityPolicies.TunnelingUserPolicy)
        .SetResponseStatusCode();

        
    }
}
