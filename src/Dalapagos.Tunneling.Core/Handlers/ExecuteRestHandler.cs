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
    : HandlerBase<ExecuteRestCommand, OperationResult<string?>>(tunnelingRepository, config)
{
    private const int DefaultDeleteAfterMin = 60;

    private readonly ILogger<ExecuteRestHandler> _logger = logger;

    public override async ValueTask<OperationResult<string?>> Handle(ExecuteRestCommand request, CancellationToken cancellationToken)
    {
        var device = await tunnelingRepository.RetrieveDeviceAsync(request.DeviceId, cancellationToken) 
            ?? throw new DataNotFoundException($"Information not found for device {request.DeviceId}.");
        
        ArgumentNullException.ThrowIfNull(device.HubId, nameof(device.HubId));

        var deviceGroup = await tunnelingRepository.RetrieveDeviceGroupAsync(request.OrganizationId, device.HubId.Value, cancellationToken) 
            ?? throw new DataNotFoundException($"Information not found for hub {device.HubId.Value}.");

        ArgumentNullException.ThrowIfNull(deviceGroup.ServerBaseUrl, nameof(deviceGroup.ServerBaseUrl));

        var restProtocol = device.RestProtocol ?? RestProtocol.Https;
        var restPort = GetRestPort(device.RestPort, restProtocol);

        var existingTunnels = await tunnelingProvider.GetTunnelsByDeviceIdAsync(
            device.HubId.Value, 
            request.DeviceId, 
            deviceGroup.ServerBaseUrl, 
            cancellationToken);

        var tunnel = existingTunnels.FirstOrDefault(t => t.Protocol == restProtocol.ToProtocol() && t.TunnelPort == restPort && t.AllowedIps == null) 
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

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        _logger.LogTrace(content);

        var contentTypeHeader = response.Headers.FirstOrDefault(h => h.Key.Equals("content-type", StringComparison.OrdinalIgnoreCase));
        return new OperationResult<string?>(content, true, (int)response.StatusCode, []);
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
