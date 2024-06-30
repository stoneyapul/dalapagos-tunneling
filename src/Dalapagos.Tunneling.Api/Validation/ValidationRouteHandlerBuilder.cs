namespace Dalapagos.Tunneling.Api.Validation;

using Core.Model;

public static class ValidationRouteHandlerBuilder
{
    public static RouteHandlerBuilder Validate<T>(this RouteHandlerBuilder builder)
    { 
        builder.AddEndpointFilter(async (invocationContext, next) => 
        {
            var argument = invocationContext.Arguments.OfType<T>()
                .FirstOrDefault();

            if (argument == null)
            {
                return await next(invocationContext);              
            }

            var response = argument.DataAnnotationsValidate();
            if (!response.IsValid)
            {
                var errors = response.Results
                    .Select(x => x.ErrorMessage)
                    .ToArray() 
                ?? ["Validation error."];

                return TypedResults.BadRequest(new OperationResult(false, 400, errors!));
            }

            return await next(invocationContext);
        });

        return builder;
    }
}
