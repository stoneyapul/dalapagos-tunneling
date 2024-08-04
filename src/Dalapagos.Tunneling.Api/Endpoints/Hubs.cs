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

public static class Hubs
{
   public static void RegisterHubEndpoints(this IEndpointRouteBuilder routes)
    {
        var endpoints = routes.MapGroup("/organizations/{organizationId}/v1/hubs")
        .WithName("Hubs");
 
        endpoints.MapGet("/{hubId}", async (Guid organizationId, Guid hubId, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new GetHubByIdQuery(
                    hubId,
                    organizationId,
                    context.User.GetUserId()), 
                cancellationToken);

            var mapper = new HubMapper();
            return mapper.MapOperationResult(result);
        })
        .WithName("Get Hub")
        .RequireAuthorization(SecurityPolicies.TunnelingUserPolicy)
        .SetResponseStatusCode();      

        endpoints.MapPost("", async (Guid organizationId, AddHubRequest request, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new AddHubCommand(
                    request.HubId, 
                    organizationId,
                    context.User.GetUserId(),
                    request.Name,
                    Enum.Parse<ServerLocation>(request.Location, true)), 
                cancellationToken);

            var mapper = new HubMapper();
            return mapper.MapOperationResult(result);
        })
        .WithName("Add Hub")
        .Validate<AddHubRequest>()
        .RequireAuthorization(SecurityPolicies.TunnelingAdminPolicy)
        .SetResponseStatusCode();

         endpoints.MapPut("/{hubId}", async (Guid organizationId, Guid hubId, UpdateHubRequest request, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new UpdateHubCommand(
                    hubId, 
                    organizationId,
                    context.User.GetUserId(),
                    request.Name),
                cancellationToken);

            var mapper = new HubMapper();
            return mapper.MapOperationResult(result);
        })
        .WithName("Update Hub")
        .WithDescription("Update hub name.")
        .Validate<UpdateHubRequest>()
        .RequireAuthorization(SecurityPolicies.TunnelingAdminPolicy)
        .SetResponseStatusCode();

       endpoints.MapDelete("/{hubId}", async (Guid organizationId, Guid hubId, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new DeleteHubCommand(
                    hubId,
                    organizationId,
                    context.User.GetUserId()), 
                cancellationToken);

            return result;
        })
        .WithName("Delete Hub")
        .RequireAuthorization(SecurityPolicies.TunnelingAdminPolicy)
        .SetResponseStatusCode();      
    }
}
