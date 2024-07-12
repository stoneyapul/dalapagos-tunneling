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
        var endpoints = routes.MapGroup("/organizations/{organizationId}/v1/devicegroups");
 
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
        .WithName("GetDeviceGroup")
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

            var mapper = new DeviceMapper();
            return mapper.MapOperationResult(result);
        })
        .WithName("AddDeviceGroup")
        .Validate<AddDeviceGroupRequest>()
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

            var mapper = new DeviceMapper();
            return mapper.MapOperationResult(result);
        })
        .WithName("DeleteDeviceGroup")
        .RequireAuthorization(SecurityPolicies.TunnelingAdminPolicy)
        .SetResponseStatusCode();      
    }
}
