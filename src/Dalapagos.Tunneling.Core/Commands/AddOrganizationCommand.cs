namespace Dalapagos.Tunneling.Core.Commands;

using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Mediator;
using Model;

public record AddOrganizationCommand(Guid? Id, string Name) : IRequest<OperationResult<Organization>>;

public class AddOrganizationHandler(ITunnelingRepository tunnelingRepository) : IRequestHandler<AddOrganizationCommand, OperationResult<Organization>>
{
    public async ValueTask<OperationResult<Organization>> Handle(AddOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await tunnelingRepository.UpsertOrganizationAsync(
            request.Id.HasValue ? request.Id : Guid.NewGuid(), 
            request.Name, 
            cancellationToken);
            
        return new OperationResult<Organization>(organization, true, []);
    }
}
