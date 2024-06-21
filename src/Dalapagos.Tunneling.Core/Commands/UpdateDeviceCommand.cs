namespace Dalapagos.Tunneling.Core;

using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Mediator;
using Model;

public record UpdateDeviceCommand(Guid Id, Guid? DeviceGroupId, string Name) : IRequest<OperationResult<Device>>;

public class UpdateDeviceHandler(ITunnelingRepository tunnelingRepository) : IRequestHandler<UpdateDeviceCommand, OperationResult<Device>>
{
    public async ValueTask<OperationResult<Device>> Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
    {
        var device = await tunnelingRepository.UpsertDeviceAsync(
            request.Id, 
            request.DeviceGroupId,
            request.Name, 
            cancellationToken);
            
        return new OperationResult<Device>(device, true, []);
    }
}