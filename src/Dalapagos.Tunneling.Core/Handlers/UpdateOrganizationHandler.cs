namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Commands;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Model;

internal sealed class UpdateOrganizationHandler(ITunnelingRepository tunnelingRepository, IConfiguration config) 
    : HandlerBase<UpdateOrganizationCommand, OperationResult<Organization>>(tunnelingRepository, config)
{
    public override async ValueTask<OperationResult<Organization>> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);

        var organization = await tunnelingRepository.UpsertOrganizationAsync(
            request.Id, 
            request.Name, 
            cancellationToken);
            
        return new OperationResult<Organization>(organization, true, Constants.StatusSuccess, []);
    }
}
