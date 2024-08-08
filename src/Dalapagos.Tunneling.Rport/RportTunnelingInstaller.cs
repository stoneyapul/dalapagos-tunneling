namespace Dalapagos.Tunneling.Rport;

using Client;
using Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Refit;

public static class RportTunnelingInstaller
{
    public static void AddRportTunneling(this IServiceCollection services)
    {
        var refitSettings = new RefitSettings { ContentSerializer = new SystemTextJsonContentSerializer() };

        services
            .AddRefitClient<IRportPairingClient>(refitSettings)
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri("https://pairing.openrport.io/"); // TODO: Move to config
            });

        services.AddScoped<ITunnelingProvider, RportTunneling>();
    }

}
