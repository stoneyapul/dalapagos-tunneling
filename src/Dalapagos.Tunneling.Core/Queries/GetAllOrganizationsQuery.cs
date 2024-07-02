namespace Dalapagos.Tunneling.Core.Queries;

using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Mediator;
using Model;

public record GetAllOrganizationsQuery(): IRequest<OperationResult<IList<Organization>>>;

public class GetAllOrganizationsHandler(ITunnelingRepository tunnelingRepository) : IRequestHandler<GetAllOrganizationsQuery, OperationResult<IList<Organization>>>
{
    public async ValueTask<OperationResult<IList<Organization>>> Handle(GetAllOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var organizations = await tunnelingRepository.GetOrganizationsAsync(cancellationToken);       
        return new OperationResult<IList<Organization>>(organizations, true, Constants.StatusSuccess, []);
    }
}