namespace Dalapagos.Tunneling.Api.Mappers;

using Core.Model;

internal static class StatusCodeRouteHandlerBuilder
{
    public static RouteHandlerBuilder SetResponseStatusCode(this RouteHandlerBuilder builder)
    {
        builder.AddEndpointFilter(async (invocationContext, next) => 
        {
            var response = await next(invocationContext);
            if (response is not OperationResult result)
            {
                return response;
            }

            invocationContext.HttpContext.Response.StatusCode = result.StatusCode;
            return result;
        });

        return builder;
    }
}
