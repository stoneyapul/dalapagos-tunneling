namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Commands;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Model;

internal sealed class UpdateDeviceHandler(ITunnelingRepository tunnelingRepository, IConfiguration config) 
    : HandlerBase<UpdateDeviceCommand, OperationResult<Device>>(tunnelingRepository, config)
{
    public override async ValueTask<OperationResult<Device>> Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);

        var device = await tunnelingRepository.UpsertDeviceAsync(
            request.Id, 
            request.DeviceGroupId,
            request.Name, 
            request.Os,
            cancellationToken);
            
        return new OperationResult<Device>(device, true, Constants.StatusSuccess, []);
    }
}