namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Commands;
using Infrastructure;
using Model;

internal sealed class AddDeviceHandler(ITunnelingRepository tunnelingRepository) 
    : HandlerBase<AddDeviceCommand, OperationResult<Device>>(tunnelingRepository)
{
    public override async ValueTask<OperationResult<Device>> Handle(AddDeviceCommand request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);
 
        var device = await tunnelingRepository.UpsertDeviceAsync(
            request.Id.HasValue ? request.Id : Guid.NewGuid(), 
            request.DeviceGroupId,
            request.Name,
            request.Os, 
            cancellationToken);
            
        return new OperationResult<Device>(device, true, Constants.StatusSuccessCreated, []);
    }
}