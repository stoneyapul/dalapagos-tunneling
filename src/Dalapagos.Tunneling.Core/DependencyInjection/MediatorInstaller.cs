namespace Dalapagos.Tunneling.Core.DependencyInjection;

using Behaviours;
using Mediator;
using Microsoft.Extensions.DependencyInjection;

public static class MediatorInstaller
{
    public static void AddMediation(this IServiceCollection services)
    {
        services.AddMediator(opts => opts.ServiceLifetime = ServiceLifetime.Transient);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandAuthorizationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestLoggingPipelineBehaviour<,>));
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(GlobalExceptionBehaviour<,>));
    }
}
