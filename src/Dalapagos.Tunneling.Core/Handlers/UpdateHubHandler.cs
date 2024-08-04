namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Commands;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Model;

internal sealed class UpdateHubHandler(ITunnelingRepository tunnelingRepository, ITunnelingProvider tunnelingProvider, IConfiguration config) 
    : HandlerBase<UpdateHubCommand, OperationResult<Hub>>(tunnelingRepository, config)
{
    public override async ValueTask<OperationResult<Hub>> Handle(UpdateHubCommand request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);

        var server = await tunnelingProvider.GetServerInformationAsync(request.OrganizationId, request.Id.ToString(), cancellationToken);
        var deviceGroup = await tunnelingRepository.RetrieveDeviceGroupAsync(request.OrganizationId, request.Id, cancellationToken);  

        if (server.Status != deviceGroup.ServerStatus)
        {
            deviceGroup.ServerStatus = server.Status;
        }

        deviceGroup = await tunnelingRepository.UpsertDeviceGroupAsync(
            request.Id, 
            request.OrganizationId,
            request.Name, 
            deviceGroup.ServerLocation,
            deviceGroup.ServerStatus,
            deviceGroup.ServerBaseUrl,
            cancellationToken);
            
        var hub = new Hub(deviceGroup, server.ConnectedDevices);
        return new OperationResult<Hub>(hub, true, Constants.StatusSuccess, []);
    }
}