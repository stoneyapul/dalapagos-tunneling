namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Model;
using Queries;

internal sealed class GetDeviceGroupByIdHandler(ITunnelingRepository tunnelingRepository) 
    : HandlerBase<GetDeviceGroupByIdQuery, OperationResult<DeviceGroup>>(tunnelingRepository)
{
   public override async ValueTask<OperationResult<DeviceGroup>> Handle(GetDeviceGroupByIdQuery request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);
        
        var deviceGroup = await tunnelingRepository.RetrieveDeviceGroupAsync(request.OrganizationId, request.Id, cancellationToken);       
        return new OperationResult<DeviceGroup>(deviceGroup, true, Constants.StatusSuccess, []);
    }
}