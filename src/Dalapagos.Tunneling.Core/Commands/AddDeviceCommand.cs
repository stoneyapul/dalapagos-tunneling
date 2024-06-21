namespace Dalapagos.Tunneling.Core;

using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Mediator;
using Model;

public record AddDeviceCommand(Guid? Id, Guid? DeviceGroupId, string Name) : IRequest<OperationResult<Device>>;

public class AddDeviceHandler(ITunnelingRepository tunnelingRepository) : IRequestHandler<AddDeviceCommand, OperationResult<Device>>
{
    public async ValueTask<OperationResult<Device>> Handle(AddDeviceCommand request, CancellationToken cancellationToken)
    {
        var device = await tunnelingRepository.UpsertDeviceAsync(
            request.Id.HasValue ? request.Id : Guid.NewGuid(), 
            request.DeviceGroupId,
            request.Name, 
            cancellationToken);
            
        return new OperationResult<Device>(device, true, []);
    }
}