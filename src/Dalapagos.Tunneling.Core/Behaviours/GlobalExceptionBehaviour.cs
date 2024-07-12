namespace Dalapagos.Tunneling.Core.Behaviours;

using System;
using System.Threading.Tasks;
using Mediator;
using Microsoft.Extensions.Logging;
using Model;

public sealed class GlobalExceptionBehaviour<TMessage, TResponse>(ILogger<GlobalExceptionBehaviour<TMessage, TResponse>> logger) 
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : notnull, IMessage
    where TResponse : OperationResult
{
    private readonly Dictionary<string, Func<Exception, int>> exceptionHandlers =
        new()
        {
            { "NotImplementedException", notImplementedHandler },
            { "ArgumentException", badRequestHandler },
            { "ArgumentNullException", badRequestHandler },
            { "AccessDeniedException", unauthorizedHandler },
            { "DataNotFoundException", notFoundHandler }
        };

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
            
            if (ex is AggregateException exception1)
            {
                var exceptions = exception1.Flatten().InnerExceptions;
                ex = exceptions.First();
            }

            var exceptionTypeName = ex.GetType().Name;

            var status = exceptionHandlers.ContainsKey(exceptionTypeName)
                ? exceptionHandlers[exceptionTypeName](ex)
                : Constants.StatusFailServer;

            return responseType.IsGenericType
                ? (Activator.CreateInstance(responseType, [default, false, status, errors]) as TResponse)!
                : (Activator.CreateInstance(responseType, [false, status, errors]) as TResponse)!;
        }
    }

    private static readonly Func<Exception, int> notImplementedHandler = (ex) =>
    {
        return Constants.StatusNotImplemented;
    };

    private static readonly Func<Exception, int> badRequestHandler = (ex) =>
    {
        return Constants.StatusFailClient;
    };

    private static readonly Func<Exception, int> unauthorizedHandler = (ex) =>
    {
        return Constants.StatusPermissionDenied;
    };

    private static readonly Func<Exception, int> notFoundHandler = (ex) =>
    {
        return Constants.StatusNotFound;
    };
}