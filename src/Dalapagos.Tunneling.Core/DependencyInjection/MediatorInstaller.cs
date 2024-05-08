namespace Dalapagos.Tunneling.Core.DependencyInjection;

using Mediator;
using Microsoft.Extensions.DependencyInjection;

public static class MediatorInstaller
{
    public static void AddMediation(this IServiceCollection services)
    {
        services.AddMediator();
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(GlobalExceptionHandler<,>));
    }
}
