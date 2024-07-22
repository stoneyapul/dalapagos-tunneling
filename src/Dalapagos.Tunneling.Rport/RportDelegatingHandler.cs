namespace Dalapagos.Tunneling.Rport;

using System.Configuration;
using Core;
using Core.Infrastructure;
using Dalapagos.Tunneling.Core.Extensions;

public class RportDelegatingHandler(
    IDeviceGroupContextAccessor deviceGroupContextAccessor, 
    ITunnelingRepository tunnelingRepository, 
    ISecrets secrets) 
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request.RequestUri);
        if(!request.RequestUri.AbsoluteUri.StartsWith(Constants.FakeBaseUrl))
        {
            throw new ConfigurationErrorsException("Invalid base Url.");
        }

        // Replace the fake URI with a URI associated with the device group.
        var baseUrl = await tunnelingRepository.RetrieveServerBaseAddressAsync(deviceGroupContextAccessor.Current.DeviceGroupId, cancellationToken);
        var relativeUrl = request.RequestUri.AbsoluteUri[Constants.FakeBaseUrl.Length..];
        request.RequestUri = new Uri(Combine(baseUrl, relativeUrl));

        // Add the Authorization header.
        var serverPassword = await secrets.GetSecretAsync($"{deviceGroupContextAccessor.Current.DeviceGroupId.ToShortDeviceGroupId()}{Constants.TunnelingServerPassNameSfx}", cancellationToken);
        var basicToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"admin:{serverPassword}"));
        request.Headers.Add("Authorization", $"Basic: {basicToken}");

        return await base.SendAsync(request, cancellationToken);
    }

    private static string Combine(string baseUrl, string relativeUrl)
    { 
        return $"{baseUrl.TrimEnd('/')}/{relativeUrl.TrimStart('/')}";
    }
}
