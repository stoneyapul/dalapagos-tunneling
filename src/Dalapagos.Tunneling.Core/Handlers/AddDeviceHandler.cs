namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Commands;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Model;

internal sealed class AddDeviceHandler(ITunnelingRepository tunnelingRepository, ITunnelingProvider tunnelingProvider, IConfiguration config) 
    : HandlerBase<AddDeviceCommand, OperationResult<Device>>(tunnelingRepository, config)
{
    public override async ValueTask<OperationResult<Device>> Handle(AddDeviceCommand request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);

        var device = await tunnelingRepository.UpsertDeviceAsync(
            request.Id.HasValue ? request.Id : Guid.NewGuid(), 
            request.DeviceGroupId,
            request.Name,
            request.Os, 
            request.OrganizationId,
            cancellationToken);
            
        return new OperationResult<Device>(device, true, Constants.StatusSuccessCreated, []);
    }
}