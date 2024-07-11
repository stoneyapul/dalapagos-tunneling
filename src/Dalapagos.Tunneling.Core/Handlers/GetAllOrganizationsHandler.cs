namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Model;
using Queries;

internal sealed class GetAllOrganizationsHandler(ITunnelingRepository tunnelingRepository, IConfiguration config)
        : HandlerBase<GetAllOrganizationsQuery, OperationResult<IList<Organization>>>(tunnelingRepository, config)
{
    public override async ValueTask<OperationResult<IList<Organization>>> Handle(GetAllOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var organizations = await tunnelingRepository.GetOrganizationsAsync(cancellationToken);  
        var organizationUsers = await GetUserOrganizationsAsync(request, cancellationToken);              

        // Return only organizations that the user is allowed to see.
        var filteredOrganizations = organizations
            .Where(o => organizationUsers.Any(ou => ou.OrganizationId == o.Id))
            .ToList();

        return new OperationResult<IList<Organization>>(filteredOrganizations, true, Constants.StatusSuccess, []);
    }
}