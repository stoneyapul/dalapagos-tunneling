namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Commands;
using Infrastructure;
using Model;

internal sealed  class DeleteOrganizationHandler(ITunnelingRepository tunnelingRepository) 
    : HandlerBase<DeleteOrganizationCommand, OperationResult>
{
    public override async ValueTask<OperationResult> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        await tunnelingRepository.DeleteOrganizationAsync(request.Id, cancellationToken);           
        return new OperationResult(true, Constants.StatusSuccess, []);
    }
}