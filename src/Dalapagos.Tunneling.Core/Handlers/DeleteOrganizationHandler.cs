namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Commands;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Model;

internal sealed  class DeleteOrganizationHandler(ITunnelingRepository tunnelingRepository, IConfiguration config) 
    : HandlerBase<DeleteOrganizationCommand, OperationResult>(tunnelingRepository, config)
{
    public override async ValueTask<OperationResult> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);
        
        await tunnelingRepository.DeleteOrganizationAsync(request.Id, cancellationToken);           
        return new OperationResult(true, Constants.StatusSuccess, []);
    }
}