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

    //    endpoints.MapGet("/{deviceId}/connection-status", async (Guid organizationId, Guid deviceId, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
    //     {
    //         var result = await mediator.Send(
    //             new GetDeviceGroupByIdQuery(
    //                 deviceGroupId,
    //                 organizationId,
    //                 context.User.GetUserId()), 
    //             cancellationToken);

    //         var mapper = new DeviceGroupMapper();
    //         return mapper.MapOperationResult(result);
    //     })
    //     .WithName("Get Device Connection Status")
    //     .RequireAuthorization(SecurityPolicies.TunnelingUserPolicy)
    //     .SetResponseStatusCode();      

        endpoints.MapPost("", async (Guid organizationId, AddDeviceRequest request, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new AddDeviceCommand(
                    request.DeviceId, 
                    request.HubId, 
                    request.Name, 
                    Enum.Parse<Os>(request.Os, true),
                    organizationId,
                    context.User.GetUserId()),
                cancellationToken);

            var mapper = new DeviceMapper();
            return mapper.MapOperationResult(result);
        })
        .WithName("Add Device")
        .WithTags("Devices")
        .WithOpenApi(op =>
        {
            op.Description = "Add a device.";
            op.Parameters[0].Description = "A globally unique identifier that represents the organization.";
            return op;
        })
        .Validate<AddDeviceRequest>()
        .RequireAuthorization(SecurityPolicies.TunnelingAdminPolicy)
        .SetResponseStatusCode();

        endpoints.MapPut("/{deviceId}", async (Guid organizationId, Guid deviceId, UpdateDeviceRequest request, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new UpdateDeviceCommand(
                    deviceId, 
                    request.HubId, 
                    request.Name, 
                    Enum.Parse<Os>(request.Os, true),
                    organizationId,
                    context.User.GetUserId()),
                cancellationToken);

            var mapper = new DeviceMapper();
            return mapper.MapOperationResult(result);
        })
        .WithName("Update Device")
        .WithTags("Devices")
        .WithOpenApi(op =>
        {
            op.Description = "Update a device.";
            op.Parameters[0].Description = "A globally unique identifier that represents the organization.";
            op.Parameters[1].Description = "A globally unique identifier that represents the device.";
            return op;
        })
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
        .WithTags("Devices")
        .WithOpenApi(op =>
        {
            op.Description = "Delete a device.";
            op.Parameters[0].Description = "A globally unique identifier that represents the organization.";
            op.Parameters[1].Description = "A globally unique identifier that represents the device.";
            return op;
        })
        .RequireAuthorization(SecurityPolicies.TunnelingAdminPolicy)
        .SetResponseStatusCode();      
     }
}
