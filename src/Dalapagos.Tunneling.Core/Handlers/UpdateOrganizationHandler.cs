namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Commands;
using Infrastructure;
using Model;

internal sealed class UpdateOrganizationHandler(ITunnelingRepository tunnelingRepository) 
    : HandlerBase<UpdateOrganizationCommand, OperationResult<Organization>>
{
    public override async ValueTask<OperationResult<Organization>> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await tunnelingRepository.UpsertOrganizationAsync(
            request.Id, 
            request.Name, 
            cancellationToken);
            
        return new OperationResult<Organization>(organization, true, Constants.StatusSuccess, []);
    }
}
