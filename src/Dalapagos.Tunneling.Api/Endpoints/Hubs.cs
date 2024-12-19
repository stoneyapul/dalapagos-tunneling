namespace Dalapagos.Tunneling.Api.Endpoints;

using Core.Commands;
using Core.Extensions;
using Core.Model;
using Core.Queries;
using Dto;
using Mappers;
using Mediator;
using Security;
using Validation;

public static class Hubs
{
   public static void RegisterHubEndpoints(this IEndpointRouteBuilder routes)
    {
        var endpoints = routes.MapGroup("/v1/organizations/{organizationId}/hubs")
            .WithName("Hubs");
 
        endpoints.MapGet("", async (Guid organizationId, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new GetHubsByOrganizationIdQuery(
                    organizationId,
                    context.User.GetUserId()), 
                cancellationToken);

            var mapper = new HubsMapper();
            return mapper.MapOperationResult(result);
        })
        .WithName("Get Hubs")
        .WithTags("Hubs")
        .WithOpenApi(op =>
        {
            op.Description = "Get a list of hubs by organization.";
            op.Parameters[0].Description = "A globally unique identifier that represents the organization.";
            return op;
        })
        .RequireAuthorization(SecurityPolicies.TunnelingUserPolicy)
        .SetResponseStatusCode();      

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
        .WithTags("Hubs")
        .WithOpenApi(op =>
        {
            op.Description = "Get a hub.";
            op.Parameters[0].Description = "A globally unique identifier that represents the organization.";
            op.Parameters[1].Description = "A globally unique identifier that represents the hub.";
            return op;
        })
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
        .WithTags("Hubs")
        .WithOpenApi(op =>
        {
            op.Description = "Add a hub.";
            op.Parameters[0].Description = "A globally unique identifier that represents the organization.";
            return op;
        })
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
        .WithTags("Hubs")
        .WithOpenApi(op =>
        {
            op.Description = "Update a hub.";
            op.Parameters[0].Description = "A globally unique identifier that represents the organization.";
            op.Parameters[1].Description = "A globally unique identifier that represents the hub.";
            return op;
        })
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
        .WithTags("Hubs")
        .WithOpenApi(op =>
        {
            op.Description = "Delete a hub.";
            op.Parameters[0].Description = "A globally unique identifier that represents the organization.";
            op.Parameters[1].Description = "A globally unique identifier that represents the hub.";
            return op;
        })
        .RequireAuthorization(SecurityPolicies.TunnelingAdminPolicy)
        .SetResponseStatusCode();      
    }
}
