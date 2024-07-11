namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Commands;
using Infrastructure;
using Model;

internal sealed class DeleteDeviceHandler(ITunnelingRepository tunnelingRepository)
    : HandlerBase<DeleteDeviceCommand, OperationResult>(tunnelingRepository)
{
    public override async ValueTask<OperationResult> Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);
        
        await tunnelingRepository.DeleteDeviceAsync(request.Id, cancellationToken);           
        return new OperationResult(true, Constants.StatusSuccess, []);
    }
}