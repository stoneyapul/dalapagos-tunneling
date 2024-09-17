namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Model;
using Queries;

internal sealed class GetOrganizationsByUserIdHandler(ITunnelingRepository tunnelingRepository, IConfiguration config)
    : HandlerBase<GetOrganizationsByUserIdQuery, OperationResult<IList<Organization>>>(tunnelingRepository, config)
{
    public override async ValueTask<OperationResult<IList<Organization>>> Handle(GetOrganizationsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var organizationUsers = await tunnelingRepository.GetOrganizationUsersByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new DataNotFoundException($"User {request.UserId} not found.");

        var organizations = new List<Organization>();
        foreach (var organizationUser in organizationUsers)
        {
            if (!organizations.Any(o => o.Id == organizationUser.OrganizationId))
            {
                organizations.Add(new Organization(organizationUser.OrganizationId, organizationUser.OrganizationName, null));
            }
        }

        return new OperationResult<IList<Organization>>(organizations, true, Constants.StatusSuccess, []);
    }
}