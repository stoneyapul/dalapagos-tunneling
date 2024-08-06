namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Commands;
using Exceptions;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;

internal sealed class AddDeviceHandler(
    ITunnelingRepository tunnelingRepository, 
    ITunnelingProvider tunnelingProvider, 
    IConfiguration config, 
    ILogger<AddDeviceHandler> logger) 
    : HandlerBase<AddDeviceCommand, OperationResult<Device>>(tunnelingRepository, config)
{
    public override async ValueTask<OperationResult<Device>> Handle(AddDeviceCommand request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);

        var deviceId = request.Id ?? Guid.NewGuid();
        var device = await tunnelingRepository.UpsertDeviceAsync(
            deviceId, 
            request.HubId,
            request.Name,
            request.Os, 
            request.OrganizationId,
            cancellationToken);
            
        // Specifying a hub that the device belongs to is optional.    
        if (!request.HubId.HasValue)
        {
            return new OperationResult<Device>(device, true, Constants.StatusSuccessCreated, []);
        }

        try
        {
            var deviceGroup = await tunnelingRepository.RetrieveDeviceGroupAsync(request.OrganizationId, request.HubId.Value, cancellationToken) 
                ?? throw new DataNotFoundException($"Information not found for hub {request.HubId.Value}.");

            ArgumentNullException.ThrowIfNull(deviceGroup.ServerBaseUrl, nameof(deviceGroup.ServerBaseUrl));
 
            var credentialString = $"{deviceId}:{CreatePassword()}";
            await tunnelingProvider.AddDeviceCredentialStringAsync(request.HubId.Value, deviceId, deviceGroup.ServerBaseUrl, credentialString, cancellationToken);           
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex.Message);
        }

        return new OperationResult<Device>(device, true, Constants.StatusSuccessCreated, []);
    }
}