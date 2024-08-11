namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Model;
using Queries;

internal sealed class GetDeviceConnectionStatusHandler(ITunnelingRepository tunnelingRepository, ITunnelingProvider tunnelingProvider, IConfiguration config)
    : HandlerBase<GetDeviceConnectionStatusQuery, OperationResult<bool>>(tunnelingRepository, config)
{
    public override async ValueTask<OperationResult<bool>> Handle(GetDeviceConnectionStatusQuery request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);
        
        var device = await tunnelingRepository.RetrieveDeviceAsync(request.Id, cancellationToken) 
            ?? throw new DataNotFoundException($"Information not found for device {request.Id}.");

        ArgumentNullException.ThrowIfNull(device.HubId, nameof(device.HubId));

        var deviceGroup = await tunnelingRepository.RetrieveDeviceGroupAsync(request.OrganizationId, device.HubId.Value, cancellationToken) 
            ?? throw new DataNotFoundException($"Information not found for hub {device.HubId.Value}.");

        ArgumentNullException.ThrowIfNull(deviceGroup.ServerBaseUrl, nameof(deviceGroup.ServerBaseUrl));

        var isConnected = await tunnelingProvider.IsDeviceConnectedAsync(
            device.HubId.Value, 
            request.Id, 
            deviceGroup.ServerBaseUrl, 
            cancellationToken);

        return new OperationResult<bool>(isConnected, true, Constants.StatusSuccess, []);;
    }
}
