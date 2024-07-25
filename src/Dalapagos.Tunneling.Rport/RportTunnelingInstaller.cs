namespace Dalapagos.Tunneling.Rport;

using Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

public static class RportTunnelingInstaller
{
    public static void AddRportTunneling(this IServiceCollection services)
    {
        services.AddScoped<ITunnelingProvider, RportTunneling>();
    }
}
