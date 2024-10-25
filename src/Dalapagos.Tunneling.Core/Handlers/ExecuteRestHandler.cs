namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Commands;
using Exceptions;
using Extensions;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Model;
using Refit;

internal sealed class ExecuteRestHandler(ITunnelingRepository tunnelingRepository, IConfiguration config, ITunnelingProvider tunnelingProvider)
    : HandlerBase<ExecuteRestCommand, OperationResult<string?>>(tunnelingRepository, config)
{
    private const int DefaultDeleteAfterMin = 60;

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

        var tunnel = await tunnelingProvider.AddTunnelAsync(
            device.HubId.Value,
            request.DeviceId,
            deviceGroup.ServerBaseUrl,
            restProtocol.ToProtocol(),
            restPort,
            DefaultDeleteAfterMin,
            null,
            cancellationToken);

        ArgumentNullException.ThrowIfNull(tunnel.Url, nameof(tunnel.Url));

        var restClient = CreateClient(tunnel.Url);

        var response = request.Action switch
        {
            "GET" => await restClient.Get(request.Action, cancellationToken),
            "POST" => await restClient.Post(request.Action, cancellationToken),
            "PUT" => await restClient.Put(request.Action, cancellationToken),
            "PATCH" => await restClient.Patch(request.Action, cancellationToken),
            "DELETE" => await restClient.Delete(request.Action, cancellationToken),
            _ => throw new Exception($"Invalid action {request.Action}."),
        };

        var contentTypeHeader = response.Headers.FirstOrDefault(h => h.Key.Equals("content-type", StringComparison.OrdinalIgnoreCase));
        return new OperationResult<string?>(response.ReasonPhrase, true, (int)response.StatusCode, []); // TODO: Return response content
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
