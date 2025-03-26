namespace Dalapagos.Tunneling.Core.Behaviours;

using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.Extensions.Logging;
using Model;

public sealed class RequestLoggingPipelineBehaviour<TMessage, TResponse>(ILogger<RequestLoggingPipelineBehaviour<TMessage, TResponse>> logger) 
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
    where TResponse : OperationResult
{
    public async ValueTask<TResponse> Handle(TMessage message, CancellationToken cancellationToken, MessageHandlerDelegate<TMessage, TResponse> next)
    {
        var requestName = typeof(TMessage).Name;
        logger.LogInformation("Handling {RequestName}", requestName);

        var response = await next(message, cancellationToken);
        if (response.IsSuccessful)
        {
            logger.LogInformation("Handled {RequestName}", requestName);
        }
        else
        {
            logger.LogError("Handled {RequestName} with errors: {Errors}", requestName, response.Errors);
        }

        return response;
    }
}
