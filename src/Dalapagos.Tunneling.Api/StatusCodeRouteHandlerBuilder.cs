namespace Dalapagos.Tunneling.Api;

using Core.Model;

public static class StatusCodeRouteHandlerBuilder
{
    public static RouteHandlerBuilder SetStatusCode(this RouteHandlerBuilder builder)
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
