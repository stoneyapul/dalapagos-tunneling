namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Commands;
using Exceptions;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;

internal sealed class DeleteDeviceHandler(
    ITunnelingRepository tunnelingRepository, 
    ITunnelingProvider tunnelingProvider, 
    IConfiguration config, 
    ILogger<DeleteDeviceHandler> logger)
    : HandlerBase<DeleteDeviceCommand, OperationResult>(tunnelingRepository, config)
{
    public override async ValueTask<OperationResult> Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);
        
        var device = await tunnelingRepository.RetrieveDeviceAsync(request.Id, cancellationToken) 
            ?? throw new DataNotFoundException($"Information not found for device {request.Id}.");

        await tunnelingRepository.DeleteDeviceAsync(request.Id, cancellationToken);           

        logger.LogInformation("Device {DeviceId} information was deleted from the database.", request.Id);

        if (!device.HubId.HasValue)
        {
            return new OperationResult(true, Constants.StatusSuccess, []);
        }

        var deviceGroup = await tunnelingRepository.RetrieveDeviceGroupAsync(request.OrganizationId, device.HubId.Value, cancellationToken) 
            ?? throw new DataNotFoundException($"Information not found for hub {device.HubId.Value}.");

        ArgumentNullException.ThrowIfNull(deviceGroup.ServerBaseUrl, nameof(deviceGroup.ServerBaseUrl));

        await tunnelingProvider.RemoveDeviceCredentialsAsync(device.HubId.Value, request.Id, deviceGroup.ServerBaseUrl, cancellationToken);

        logger.LogInformation("Device {DeviceId} was removed from the hub.", request.Id);

        return new OperationResult(true, Constants.StatusSuccess, []);
    }
}