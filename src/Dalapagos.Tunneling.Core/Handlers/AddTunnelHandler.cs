namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Commands;
using Exceptions;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Model;

internal sealed class AddTunnelHandler(ITunnelingRepository tunnelingRepository, IConfiguration config, ITunnelingProvider tunnelingProvider)
    : HandlerBase<AddTunnelCommand, OperationResult<Tunnel>>(tunnelingRepository, config)
{
    public override async ValueTask<OperationResult<Tunnel>> Handle(AddTunnelCommand request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);

        var device = await tunnelingRepository.RetrieveDeviceAsync(request.DeviceId, cancellationToken) 
            ?? throw new DataNotFoundException($"Information not found for device {request.DeviceId}.");

        ArgumentNullException.ThrowIfNull(device.HubId, nameof(device.HubId));

        var deviceGroup = await tunnelingRepository.RetrieveDeviceGroupAsync(request.OrganizationId, device.HubId.Value, cancellationToken) 
            ?? throw new DataNotFoundException($"Information not found for hub {device.HubId.Value}.");

        ArgumentNullException.ThrowIfNull(deviceGroup.ServerBaseUrl, nameof(deviceGroup.ServerBaseUrl));

        var tunnel = await tunnelingProvider.AddTunnelAsync(
            device.HubId.Value,
            request.DeviceId,
            deviceGroup.ServerBaseUrl,
            request.Protocol,
            GetPort(request.DevicePort, request.Protocol),
            request.DeleteAfterMin,
            request.AllowedIp,
            cancellationToken);

        return new OperationResult<Tunnel>(tunnel, true, Constants.StatusSuccessCreated, []);
    }

    private static ushort GetPort(ushort? port, Protocol protocol)
    {
        if (port.HasValue)
        {
            return port.Value;
        }

        return protocol switch
        {
            Protocol.Http => Constants.DefaultHttpPort,
            Protocol.Https => Constants.DefaultHttpsPort,
            Protocol.Ssh => Constants.DefaultSshPort,
            _ => throw new Exception($"Invalid protocol {protocol}."),
        };
    }
}
