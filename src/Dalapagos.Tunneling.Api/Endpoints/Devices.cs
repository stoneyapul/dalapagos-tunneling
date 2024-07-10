namespace Dalapagos.Tunneling.Api.Endpoints;

using Core.Commands;
using Core.Model;
using Dto;
using Mappers;
using Mediator;
using Security;
using Validation;

public static class Devices
{
    public static void RegisterDeviceEndpoints(this IEndpointRouteBuilder routes)
    {
        var endpoints = routes.MapGroup("v1/devices")
            .RequireAuthorization(Policies.TunnelingAdminPolicy);

        endpoints.MapPost("", async (AddDeviceRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new AddDeviceCommand(
                    request.DeviceId, 
                    request.DeviceGroupId, 
                    request.Name, 
                    Enum.Parse<Os>(request.Os, true),
                    request.OrganizationId),
                cancellationToken);

            var mapper = new DeviceMapper();
            return mapper.MapOperationResult(result);
        })
        .WithName("AddDevice")
        .Validate<AddDeviceRequest>()
        .SetStatusCode();
    }
}
