namespace Dalapagos.Tunneling.Api.Endpoints;

using Core.Commands;
using Core.Model;
using Core.Queries;
using Dto;
using Extensions;
using Mappers;
using Mediator;
using Security;
using Validation;

public static class DeviceGroups
{
   public static void RegisterDeviceGroupEndpoints(this IEndpointRouteBuilder routes)
    {
        var endpoints = routes.MapGroup("/organizations/{organizationId}/v1/devicegroups")
        .WithName("Device Groups");
 
        endpoints.MapGet("/{deviceGroupId}", async (Guid organizationId, Guid deviceGroupId, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new GetDeviceGroupByIdQuery(
                    deviceGroupId,
                    organizationId,
                    context.User.GetUserId()), 
                cancellationToken);

            var mapper = new DeviceGroupMapper();
            return mapper.MapOperationResult(result);
        })
        .WithName("Get Device Group")
        .RequireAuthorization(SecurityPolicies.TunnelingUserPolicy)
        .SetResponseStatusCode();      

        endpoints.MapPost("", async (Guid organizationId, AddDeviceGroupRequest request, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new AddDeviceGroupCommand(
                    request.DeviceGroupId, 
                    organizationId,
                    context.User.GetUserId(),
                    request.Name,
                    Enum.Parse<ServerLocation>(request.Location, true)), 
                cancellationToken);

            var mapper = new DeviceGroupMapper();
            return mapper.MapOperationResult(result);
        })
        .WithName("Add Device Group")
        .Validate<AddDeviceGroupRequest>()
        .RequireAuthorization(SecurityPolicies.TunnelingAdminPolicy)
        .SetResponseStatusCode();

         endpoints.MapPut("/{deviceGroupId}", async (Guid organizationId, Guid deviceGroupId, UpdateDeviceGroupRequest request, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new UpdateDeviceGroupCommand(
                    deviceGroupId, 
                    organizationId,
                    context.User.GetUserId(),
                    request.Name),
                cancellationToken);

            var mapper = new DeviceGroupMapper();
            return mapper.MapOperationResult(result);
        })
        .WithName("Update Device Group")
        .WithDescription("Update device group name.")
        .Validate<UpdateDeviceGroupRequest>()
        .RequireAuthorization(SecurityPolicies.TunnelingAdminPolicy)
        .SetResponseStatusCode();

       endpoints.MapDelete("/{deviceGroupId}", async (Guid organizationId, Guid deviceGroupId, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new DeleteDeviceGroupCommand(
                    deviceGroupId,
                    organizationId,
                    context.User.GetUserId()), 
                cancellationToken);

            return result;
        })
        .WithName("Delete Device Group")
        .RequireAuthorization(SecurityPolicies.TunnelingAdminPolicy)
        .SetResponseStatusCode();      
    }
}
