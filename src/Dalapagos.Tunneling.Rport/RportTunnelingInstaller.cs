namespace Dalapagos.Tunneling.Rport;

using Client;
using Core;
using Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Refit;

public static class RportTunnelingInstaller
{
    private const int MaxRetries = 2;

    public static void AddRportTunneling(this IServiceCollection services)
    {
        var refitSettings = new RefitSettings { ContentSerializer = new SystemTextJsonContentSerializer() };

        // services.AddSingleton<IDeviceGroupContextAccessor, DeviceGroupContextAccessor>();
        // services.AddSingleton<RportDelegatingHandler>();

        services
            .AddRefitClient<IRportTunnelClient>(refitSettings)
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(Constants.FakeBaseUrl);
            })
            .AddHttpMessageHandler<RportDelegatingHandler>()
            .AddPolicyHandler(GetRetryPolicy());

        services.AddScoped<ITunnelingProvider, RportTunneling>();
    }

    private static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                MaxRetries,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (result, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine(
                        $"A non success code {(int?)result.Result?.StatusCode} was received on retry {retryCount}."
                    );
                }
            );
    }
}
