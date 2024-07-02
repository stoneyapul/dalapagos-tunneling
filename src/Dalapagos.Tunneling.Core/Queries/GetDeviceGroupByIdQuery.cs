namespace Dalapagos.Tunneling.Core;

using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Mediator;
using Model;

public record GetDeviceGroupByIdQuery(Guid Id, Guid OganizationId) : IRequest<OperationResult<DeviceGroup>>;

public class GetDeviceGroupByIdHandler(ITunnelingRepository tunnelingRepository) : IRequestHandler<GetDeviceGroupByIdQuery, OperationResult<DeviceGroup>>
{
   public async ValueTask<OperationResult<DeviceGroup>> Handle(GetDeviceGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var deviceGroup = await tunnelingRepository.RetrieveDeviceGroupAsync(request.OganizationId, request.Id, cancellationToken);       
        return new OperationResult<DeviceGroup>(deviceGroup, true, Constants.StatusSuccess, []);
    }
}