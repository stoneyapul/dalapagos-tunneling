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
            request.DevicePort ?? (request.Protocol == Protocol.Ssh ? Constants.DefaultSshPort : Constants.DefaultHttpsPort),
            request.DeleteAfterMin,
            request.AllowedIp,
            cancellationToken);

        return new OperationResult<Tunnel>(tunnel, true, Constants.StatusSuccessCreated, []);
    }
}
