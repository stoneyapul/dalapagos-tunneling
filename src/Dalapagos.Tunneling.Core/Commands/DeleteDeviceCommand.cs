namespace Dalapagos.Tunneling.Core.Commands;

using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Mediator;
using Model;

public record DeleteDeviceCommand (Guid Id) : IRequest<OperationResult>;

internal sealed class DeleteDeviceHandler(ITunnelingRepository tunnelingRepository)
    : CommandBase, IRequestHandler<DeleteDeviceCommand, OperationResult>
{
    public async ValueTask<OperationResult> Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
    {
        await tunnelingRepository.DeleteDeviceAsync(request.Id, cancellationToken);           
        return new OperationResult(true, Constants.StatusSuccess, []);
    }
}