namespace Dalapagos.Tunneling.Core.Commands;

using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Mediator;
using Model;

public record class UpdateOrganizationCommand(Guid Id, string Name) : IRequest<OperationResult<Organization>>;

public class UpdateOrganizationHandler(ITunnelingRepository tunnelingRepository) : IRequestHandler<UpdateOrganizationCommand, OperationResult<Organization>>
{
    public async ValueTask<OperationResult<Organization>> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await tunnelingRepository.UpsertOrganizationAsync(
            request.Id, 
            request.Name, 
            cancellationToken);
            
        return new OperationResult<Organization>(organization, true, []);
    }
}