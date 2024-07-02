namespace Dalapagos.Tunneling.Core.Commands;

using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Mediator;
using Model;

public record UpdateDeviceGroupCommand(Guid OrganizationId, Guid Id, string Name, ServerStatus ServerStatus) : IRequest<OperationResult<DeviceGroup>>;

public class UpdateDeviceGroupHandler(ITunnelingRepository tunnelingRepository) : IRequestHandler<UpdateDeviceGroupCommand, OperationResult<DeviceGroup>>
{
    public async ValueTask<OperationResult<DeviceGroup>> Handle(UpdateDeviceGroupCommand request, CancellationToken cancellationToken)
    {
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