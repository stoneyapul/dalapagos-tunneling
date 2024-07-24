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
        var endpoints = routes.MapGroup("/organizations/{organizationId}/v1/tunnels")
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
        .WithDescription("Create a tunnel to a device.")
        .Validate<AddTunnelRequest>()
        .RequireAuthorization(SecurityPolicies.TunnelingUserPolicy)
        .SetResponseStatusCode();

        
    }
}
