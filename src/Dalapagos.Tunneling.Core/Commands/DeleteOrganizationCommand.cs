namespace Dalapagos.Tunneling.Core.Commands;

using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Mediator;
using Model;

public record DeleteOrganizationCommand(Guid Id) : IRequest<OperationResult>;

public class DeleteOrganizationHandler(ITunnelingRepository tunnelingRepository) : IRequestHandler<DeleteOrganizationCommand, OperationResult>
{
    public async ValueTask<OperationResult> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        await tunnelingRepository.DeleteOrganizationAsync(request.Id, cancellationToken);           
        return new OperationResult(true, []);
    }
}