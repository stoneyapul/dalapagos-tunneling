namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Commands;
using Infrastructure;
using Model;

internal sealed class UpdateDeviceGroupHandler(ITunnelingRepository tunnelingRepository) 
    : HandlerBase<UpdateDeviceGroupCommand, OperationResult<DeviceGroup>>(tunnelingRepository)
{
    public override async ValueTask<OperationResult<DeviceGroup>> Handle(UpdateDeviceGroupCommand request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);

        var deviceGroup = await tunnelingRepository.RetrieveDeviceGroupAsync(request.OrganizationId, request.Id, cancellationToken);  
        deviceGroup = await tunnelingRepository.UpsertDeviceGroupAsync(
            request.Id, 
            request.OrganizationId,
            request.Name, 
            deviceGroup.ServerLocation,
            request.ServerStatus,
            cancellationToken);
            
        return new OperationResult<DeviceGroup>(deviceGroup, true, Constants.StatusSuccess, []);
    }
}