namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Model;
using Queries;

internal sealed class GetOrganizationByIdHandler(ITunnelingRepository tunnelingRepository, IConfiguration config) 
    : HandlerBase<GetOrganizationByIdQuery, OperationResult<Organization>>(tunnelingRepository, config)
{
   public override async ValueTask<OperationResult<Organization>> Handle(GetOrganizationByIdQuery request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);
        
        var organization = await tunnelingRepository.RetrieveOrganizationAsync(request.Id, cancellationToken);       
        return new OperationResult<Organization>(organization, true, Constants.StatusSuccess, []);
    }
}