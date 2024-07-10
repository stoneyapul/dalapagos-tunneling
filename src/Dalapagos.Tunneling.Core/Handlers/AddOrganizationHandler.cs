namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Commands;
using Infrastructure;
using Model;

internal sealed class AddOrganizationHandler(ITunnelingRepository tunnelingRepository) 
    : HandlerBase<AddOrganizationCommand, OperationResult<Organization>>
{
    public override async ValueTask<OperationResult<Organization>> Handle(AddOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await tunnelingRepository.UpsertOrganizationAsync(
            request.Id.HasValue ? request.Id : Guid.NewGuid(), 
            request.Name, 
            cancellationToken);
            
        return new OperationResult<Organization>(organization, true, Constants.StatusSuccessCreated, []);
    }
}