namespace Dalapagos.Tunneling.Api.Endpoints;

using System.Security.Claims;
using Core.Commands;
using Core.Model;
using Dto;
using Mappers;
using Mediator;
using Microsoft.Identity.Web;
using Security;
using Validation;

public static class Devices
{
    public static void RegisterDeviceEndpoints(this IEndpointRouteBuilder routes)
    {
        var endpoints = routes.MapGroup("v1/devices")
            .RequireAuthorization(Policies.TunnelingAdminPolicy);

        endpoints.MapPost("", async (AddDeviceRequest request, IMediator mediator, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            if (!Guid.TryParse(user.GetObjectId(), out Guid oid))
            {
                return new OperationResult(false, 401, ["User id is missing or invalid."]);
            } 

            var result = await mediator.Send(
                new AddDeviceCommand(
                    request.DeviceId, 
                    request.DeviceGroupId, 
                    request.Name, 
                    Enum.Parse<Os>(request.Os, true),
                    request.OrganizationId,
                    oid),
                cancellationToken);

            var mapper = new DeviceMapper();
            return mapper.MapOperationResult(result);
        })
        .WithName("AddDevice")
        .Validate<AddDeviceRequest>()
        .SetStatusCode();
    }
}
