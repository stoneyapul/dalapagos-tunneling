namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using Infrastructure;
using Queries;
using Microsoft.Extensions.Configuration;
using Model;

internal sealed class GetDevicePairingScriptHandler(
    ITunnelingRepository tunnelingRepository,
    ITunnelingProvider tunnelingProvider,
    IConfiguration config)
    : HandlerBase<GetDevicePairingScriptQuery, OperationResult<string?>>(tunnelingRepository, config)
{
    public override async ValueTask<OperationResult<string?>> Handle(GetDevicePairingScriptQuery request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);

        var device = await tunnelingRepository.RetrieveDeviceAsync(request.Id, cancellationToken) 
            ?? throw new DataNotFoundException($"Information not found for device {request.Id}.");

        ArgumentNullException.ThrowIfNull(device.HubId, nameof(device.HubId));

        var deviceGroup = await tunnelingRepository.RetrieveDeviceGroupAsync(request.OrganizationId, device.HubId.Value, cancellationToken) 
            ?? throw new DataNotFoundException($"Information not found for hub {device.HubId.Value}.");

        ArgumentNullException.ThrowIfNull(deviceGroup.ServerBaseUrl, nameof(deviceGroup.ServerBaseUrl));

        var pairingScript = await tunnelingProvider.GetPairingScriptAsync(
            device.HubId.Value,
            request.Id,
            deviceGroup.ServerBaseUrl,
            device.Os,
            cancellationToken);

        return new OperationResult<string?>(pairingScript, true, Constants.StatusSuccess, []);
    }
}
