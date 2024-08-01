namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Model;
using Queries;

internal sealed class GetDeviceConnectionStatusHandler(ITunnelingRepository tunnelingRepository, ITunnelingProvider tunnelingProvider, IConfiguration config)
    : HandlerBase<GetDeviceConnectionStatusQuery, OperationResult<bool>>(tunnelingRepository, config)
{
    public override async ValueTask<OperationResult<bool>> Handle(GetDeviceConnectionStatusQuery request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);
        
        // TODO: Implement this stuff.
        var device = await tunnelingRepository.RetrieveDeviceAsync(request.Id, cancellationToken);

        return new OperationResult<bool>(false, true, Constants.StatusSuccess, []);;
    }
}
