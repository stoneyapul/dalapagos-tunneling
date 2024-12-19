namespace Dalapagos.Tunneling.Api.Endpoints;

using Core.Commands;
using Core.Extensions;
using Core.Model;
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
                    path,
                    organizationId,
                    context.User.GetUserId()),
                cancellationToken);

            return await GetResultAsync(result, cancellationToken);
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
            var result = await mediator.Send(
                new ExecuteRestCommand(
                    deviceId,
                    "POST",
                    path,
                    organizationId,
                    context.User.GetUserId()),
                cancellationToken);

            return await GetResultAsync(result, cancellationToken);
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
            var result = await mediator.Send(
                new ExecuteRestCommand(
                    deviceId,
                    "PUT",
                    path,
                    organizationId,
                    context.User.GetUserId()),
                cancellationToken);

            return await GetResultAsync(result, cancellationToken);
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
            var result = await mediator.Send(
                new ExecuteRestCommand(
                    deviceId,
                    "PATCH",
                    path,
                    organizationId,
                    context.User.GetUserId()),
                cancellationToken);
    
            return await GetResultAsync(result, cancellationToken);
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
            var result = await mediator.Send(
                new ExecuteRestCommand(
                    deviceId,
                    "DELETE",
                    path,
                    organizationId,
                    context.User.GetUserId()),
                cancellationToken);

            return await GetResultAsync(result, cancellationToken);
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

    private static async Task<IResult> GetResultAsync(OperationResult<HttpResponseMessage?> result, CancellationToken cancellationToken)
    {
            if (result.Data == null)
            {
                return Results.StatusCode(result.StatusCode);
            }

            var content = await result.Data.Content.ReadAsStringAsync(cancellationToken);
            var contentType = result.Data.Content.Headers.ContentType?.MediaType;

            return Results.Text(content, contentType: contentType, statusCode: result.StatusCode);
    }
}