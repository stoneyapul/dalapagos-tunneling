namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Commands;
using Exceptions;
using Extensions;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;
using Refit;

internal sealed class ExecuteRestHandler(
    ITunnelingRepository tunnelingRepository, 
    IConfiguration config, 
    ILogger<ExecuteRestHandler> logger, 
    ITunnelingProvider tunnelingProvider)
    : HandlerBase<ExecuteRestCommand, OperationResult<HttpResponseMessage?>>(tunnelingRepository, config)
{
    private const int DefaultDeleteAfterMin = 60;

    private readonly ILogger<ExecuteRestHandler> _logger = logger;

    public override async ValueTask<OperationResult<HttpResponseMessage?>> Handle(ExecuteRestCommand request, CancellationToken cancellationToken)
    {
        var device = await tunnelingRepository.RetrieveDeviceAsync(request.DeviceId, cancellationToken) 
            ?? throw new DataNotFoundException($"Information not found for device {request.DeviceId}.");
        
        ArgumentNullException.ThrowIfNull(device.HubId, nameof(device.HubId));

        var deviceGroup = await tunnelingRepository.RetrieveDeviceGroupAsync(request.OrganizationId, device.HubId.Value, cancellationToken) 
            ?? throw new DataNotFoundException($"Information not found for hub {device.HubId.Value}.");

        ArgumentNullException.ThrowIfNull(deviceGroup.ServerBaseUrl, nameof(deviceGroup.ServerBaseUrl));

        var restProtocol = device.RestProtocol ?? RestProtocol.Https;
        var restPort = GetRestPort(device.RestPort, restProtocol);

        _logger.LogTrace("Getting existing tunnels");

        var existingTunnels = await tunnelingProvider.GetTunnelsByDeviceIdAsync(
            device.HubId.Value, 
            request.DeviceId, 
            deviceGroup.ServerBaseUrl, 
            cancellationToken);

        _logger.LogTrace("Existing tunnels: {Tunnels}", existingTunnels.Select(t => t.Url).ToArray());

        // TODO: Should I use an Ip?
        // TODO: How to manage tunnel deletion?
        var tunnel = existingTunnels.FirstOrDefault(
            t => 
            t.Protocol == restProtocol.ToProtocol() && t.DevicePort == restPort && (t.AllowedIps == null || t.AllowedIps.Length == 0))
            ?? await tunnelingProvider.AddTunnelAsync(
                device.HubId.Value,
                request.DeviceId,
                deviceGroup.ServerBaseUrl,
                restProtocol.ToProtocol(),
                restPort,
                DefaultDeleteAfterMin,
                null,
                cancellationToken);

        _logger.LogInformation("Tunnel URL: {URL} for device {DeviceId}.", tunnel.Url, request.DeviceId);

        ArgumentNullException.ThrowIfNull(tunnel.Url, nameof(tunnel.Url));

        var restClient = CreateClient(tunnel.Url);

        var response = request.Action switch
        {
            "GET" => await restClient.Get(request.Path, cancellationToken),
            "POST" => await restClient.Post(request.Path, cancellationToken),
            "PUT" => await restClient.Put(request.Path, cancellationToken),
            "PATCH" => await restClient.Patch(request.Path, cancellationToken),
            "DELETE" => await restClient.Delete(request.Path, cancellationToken),
            _ => throw new Exception($"Invalid action {request.Action}."),
        };

        return new OperationResult<HttpResponseMessage?>(response, true, (int)response.StatusCode, []);
    }

    private static IRest CreateClient(string baseUrl)
    {
        var refitSettings = new RefitSettings { ContentSerializer = new SystemTextJsonContentSerializer() };
        var httpClient = RestService.CreateHttpClient(baseUrl, refitSettings);

        return RestService.For<IRest>(httpClient, refitSettings);
    }

    private static ushort GetRestPort(ushort? port, RestProtocol protocol)
    {
        if (port.HasValue)
        {
            return port.Value;
        }

        return protocol switch
        {
            RestProtocol.Http => Constants.DefaultHttpPort,
            RestProtocol.Https => Constants.DefaultHttpsPort,
            _ => throw new Exception($"Invalid protocol {protocol}."),
        };
    }
}
