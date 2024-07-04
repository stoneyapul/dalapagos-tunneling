namespace Dalapagos.Tunneling.Core.Queries;

using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Mediator;
using Model;

public record GetOrganizationByIdQuery(Guid Id) : IRequest<OperationResult<Organization>>;

internal sealed class GetOrganizationByIdHandler(ITunnelingRepository tunnelingRepository) : IRequestHandler<GetOrganizationByIdQuery, OperationResult<Organization>>
{
   public async ValueTask<OperationResult<Organization>> Handle(GetOrganizationByIdQuery request, CancellationToken cancellationToken)
    {
        var organization = await tunnelingRepository.RetrieveOrganizationAsync(request.Id, cancellationToken);       
        return new OperationResult<Organization>(organization, true, Constants.StatusSuccess, []);
    }
}