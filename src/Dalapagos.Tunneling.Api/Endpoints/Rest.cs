namespace Dalapagos.Tunneling.Api.Endpoints;

using Core.Commands;
using Extensions;
using Mappers;
using Mediator;
using Security;

public static class Rest
{
    public static void RegisterRestEndpoints(this IEndpointRouteBuilder routes)
    {
        var endpoints = routes.MapGroup("/v1/organizations/{organizationId}/devices/{deviceId}/rest/")
            .WithName("Rest");

        endpoints.MapGet("{*path}", async (Guid organizationId, Guid deviceId, string path, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new ExecuteRestCommand(
                    deviceId,
                    "GET",
                    organizationId,
                    context.User.GetUserId()),
                cancellationToken);

            return Results.Text(result.Data, contentType: "application/json", statusCode: result.StatusCode);
        })
        .WithName("GET Request")
        .WithTags("ReST")
        .WithOpenApi(op =>
        {
            op.Description = "Execute GET request to a device.";
            op.Parameters[0].Description = "A globally unique identifier that represents the organization.";
            op.Parameters[1].Description = "A globally unique identifier that represents the device.";
            return op;
        })
        .RequireAuthorization(SecurityPolicies.TunnelingAdminPolicy)
        .SetResponseStatusCode();

        endpoints.MapPost("{*path}", async (Guid organizationId, Guid deviceId, string path, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            return await mediator.Send(
                new ExecuteRestCommand(
                    deviceId,
                    "POST",
                    organizationId,
                    context.User.GetUserId()),
                cancellationToken);
        })
        .WithName("POST Request")
        .WithTags("ReST")
        .WithOpenApi(op =>
        {
            op.Description = "Execute POST request to a device.";
            op.Parameters[0].Description = "A globally unique identifier that represents the organization.";
            op.Parameters[1].Description = "A globally unique identifier that represents the device.";
            return op;
        })
        .RequireAuthorization(SecurityPolicies.TunnelingAdminPolicy)
        .SetResponseStatusCode();

        endpoints.MapPut("{*path}", async (Guid organizationId, Guid deviceId, string path, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            return await mediator.Send(
                new ExecuteRestCommand(
                    deviceId,
                    "PUT",
                    organizationId,
                    context.User.GetUserId()),
                cancellationToken);

        })
        .WithName("PUT Request")
        .WithTags("ReST")
        .WithOpenApi(op =>
        {
            op.Description = "Execute PUT request to a device.";
            op.Parameters[0].Description = "A globally unique identifier that represents the organization.";
            op.Parameters[1].Description = "A globally unique identifier that represents the device.";
            return op;
        })
        .RequireAuthorization(SecurityPolicies.TunnelingAdminPolicy)
        .SetResponseStatusCode();

        endpoints.MapPatch("{*path}", async (Guid organizationId, Guid deviceId, string path, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            return await mediator.Send(
                new ExecuteRestCommand(
                    deviceId,
                    "PATCH",
                    organizationId,
                    context.User.GetUserId()),
                cancellationToken);
        })        
        .WithName("PATCH Request")
        .WithTags("ReST")
        .WithOpenApi(op =>
        {
            op.Description = "Execute PATCH request to a device.";
            op.Parameters[0].Description = "A globally unique identifier that represents the organization.";
            op.Parameters[1].Description = "A globally unique identifier that represents the device.";
            return op;
        })
        .RequireAuthorization(SecurityPolicies.TunnelingAdminPolicy)
        .SetResponseStatusCode();


        endpoints.MapDelete("{*path}", async (Guid organizationId, Guid deviceId, string path, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            return await mediator.Send(
                new ExecuteRestCommand(
                    deviceId,
                    "DELETE",
                    organizationId,
                    context.User.GetUserId()),
                cancellationToken);

        })
        .WithName("DELETE Request")
        .WithTags("ReST")
        .WithOpenApi(op =>
        {
            op.Description = "Execute DELETE request to a device.";
            op.Parameters[0].Description = "A globally unique identifier that represents the organization.";
            op.Parameters[1].Description = "A globally unique identifier that represents the device.";
            return op;
        })
        .RequireAuthorization(SecurityPolicies.TunnelingAdminPolicy)
        .SetResponseStatusCode(); 
    }
}