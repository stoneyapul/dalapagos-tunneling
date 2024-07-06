namespace Dalapagos.Tunneling.Core.Behaviours;

using System;
using System.Threading.Tasks;
using Mediator;
using Microsoft.Extensions.Logging;
using Model;

public sealed class GlobalExceptionBehaviour<TMessage, TResponse>(ILogger<GlobalExceptionBehaviour<TMessage, TResponse>> logger) : IPipelineBehavior<TMessage, TResponse>
    where TMessage : notnull, IMessage
    where TResponse : OperationResult
{
    private readonly ILogger<GlobalExceptionBehaviour<TMessage, TResponse>> _logger = logger;

    public async ValueTask<TResponse> Handle(TMessage message, CancellationToken cancellationToken, MessageHandlerDelegate<TMessage, TResponse> next)
    {
        try
        {
            return await next(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Something went wrong while handling request of type {RequestType}", message.GetType());

            var responseType = typeof(TResponse);
            string[] errors = [ ex.Message ];
            
            return responseType.IsGenericType
                ? (Activator.CreateInstance(responseType, [default, false, Constants.StatusFailServer, errors]) as TResponse)!
                : (Activator.CreateInstance(responseType, [false, Constants.StatusFailServer, errors]) as TResponse)!;
        }
    }
}