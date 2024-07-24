namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Commands;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Model;

internal sealed class AddTunnelHandler(ITunnelingRepository tunnelingRepository, IConfiguration config, ITunnelingProvider tunnelingProvider)
    : HandlerBase<AddTunnelCommand, OperationResult<Tunnel>>(tunnelingRepository, config)
{
    public override async ValueTask<OperationResult<Tunnel>> Handle(AddTunnelCommand request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);

        var tunnel = await tunnelingProvider.AddTunnelAsync(
            request.DeviceId,
            request.Protocol,
            request.DevicePort ?? (request.Protocol == Protocol.Ssh ? Constants.DefaultSshPort : Constants.DefaultHttpsPort),
            request.DeleteAfterMin,
            request.AllowedIp,
            cancellationToken);

        return new OperationResult<Tunnel>(tunnel, true, Constants.StatusSuccessCreated, []);
    }
}
