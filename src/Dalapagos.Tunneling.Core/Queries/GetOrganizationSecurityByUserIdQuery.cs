namespace Dalapagos.Tunneling.Core.Queries;

using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Mediator;
using Model;

public record GetOrganizationSecurityByUserIdQuery(Guid Id) : IRequest<OperationResult<IList<OrganizationUser>>>;

internal sealed class GetOrganizationSecurityByUserIdHandler(ITunnelingRepository tunnelingRepository) : IRequestHandler<GetOrganizationSecurityByUserIdQuery, OperationResult<IList<OrganizationUser>>>
{
   public async ValueTask<OperationResult<IList<OrganizationUser>>> Handle(GetOrganizationSecurityByUserIdQuery request, CancellationToken cancellationToken)
    {
        var organizationUsers = await tunnelingRepository.GetOrganizationUsersByUserIdAsync(request.Id, cancellationToken);       
        return new OperationResult<IList<OrganizationUser>>(organizationUsers, true, Constants.StatusSuccess, []);
    }
}