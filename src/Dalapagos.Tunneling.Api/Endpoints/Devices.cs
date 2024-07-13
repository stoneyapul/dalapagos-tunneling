namespace Dalapagos.Tunneling.Api.Endpoints;

using Core.Commands;
using Core.Model;
using Dto;
using Extensions;
using Mappers;
using Mediator;
using Security;
using Validation;

public static class Devices
{
    public static void RegisterDeviceEndpoints(this IEndpointRouteBuilder routes)
    {
        var endpoints = routes.MapGroup("/organizations/{organizationId}/v1/devices")
        .WithName("Devices");

        endpoints.MapPost("", async (Guid organizationId, AddDeviceRequest request, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new AddDeviceCommand(
                    request.DeviceId, 
                    request.DeviceGroupId, 
                    request.Name, 
                    Enum.Parse<Os>(request.Os, true),
                    organizationId,
                    context.User.GetUserId()),
                cancellationToken);

            var mapper = new DeviceMapper();
            return mapper.MapOperationResult(result);
        })
        .WithName("Add Device")
        .WithDescription("Add a new device.")
        .Validate<AddDeviceRequest>()
        .RequireAuthorization(SecurityPolicies.TunnelingAdminPolicy)
        .SetResponseStatusCode();

        endpoints.MapPut("/{deviceId}", async (Guid organizationId, Guid deviceId, UpdateDeviceRequest request, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new UpdateDeviceCommand(
                    deviceId, 
                    request.DeviceGroupId, 
                    request.Name, 
                    Enum.Parse<Os>(request.Os, true),
                    organizationId,
                    context.User.GetUserId()),
                cancellationToken);

            var mapper = new DeviceMapper();
            return mapper.MapOperationResult(result);
        })
        .WithName("Update Device")
        .WithDescription("Update device information.")
        .Validate<UpdateDeviceRequest>()
        .RequireAuthorization(SecurityPolicies.TunnelingAdminPolicy)
        .SetResponseStatusCode();

        endpoints.MapDelete("/{deviceId}", async (Guid organizationId, Guid deviceId, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new DeleteDeviceCommand(
                    deviceId,
                    organizationId,
                    context.User.GetUserId()), 
                cancellationToken);

            return result;
        })
        .WithName("Delete Device")
        .WithDescription("Delete a device.")
        .RequireAuthorization(SecurityPolicies.TunnelingAdminPolicy)
        .SetResponseStatusCode();      
     }
}
