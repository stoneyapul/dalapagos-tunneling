namespace Dalapagos.Tunneling.Core.Commands;

using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Mediator;
using Model;

public record AddDeviceCommand(Guid? Id, Guid? DeviceGroupId, string Name, Os Os) : IRequest<OperationResult<Device>>;

internal sealed class AddDeviceHandler(ITunnelingRepository tunnelingRepository) : IRequestHandler<AddDeviceCommand, OperationResult<Device>>
{
    public async ValueTask<OperationResult<Device>> Handle(AddDeviceCommand request, CancellationToken cancellationToken)
    {
        var device = await tunnelingRepository.UpsertDeviceAsync(
            request.Id.HasValue ? request.Id : Guid.NewGuid(), 
            request.DeviceGroupId,
            request.Name,
            request.Os, 
            cancellationToken);
            
        return new OperationResult<Device>(device, true, Constants.StatusSuccessCreated, []);
    }
}