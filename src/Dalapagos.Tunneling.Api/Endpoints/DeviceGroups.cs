namespace Dalapagos.Tunneling.Api.Endpoints;

using Core.Commands;
using Core.Model;
using Dto;
using Mappers;
using Mediator;
using Security;

public static class DeviceGroups
{
   public static void RegisterDeviceGroupEndpoints(this IEndpointRouteBuilder routes)
    {
        var endpoints = routes.MapGroup("v1/devicegroups")
            .RequireAuthorization(Policies.TunnelingAdminPolicy);

/*         endpoints.MapPost("", async (AddDeviceGroupRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new AddDeviceGroupCommand(
                    request.DeviceGroupId, 
                    ), 
                cancellationToken);

            var mapper = new DeviceMapper();
            return mapper.MapOperationResult(result);
        })
        .WithName("AddDeviceGroup");
 */    }
}
