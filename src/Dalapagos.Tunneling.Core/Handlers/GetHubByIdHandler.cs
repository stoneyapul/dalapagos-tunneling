namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Model;
using Queries;

internal sealed class GetHubByIdHandler(ITunnelingRepository tunnelingRepository, ITunnelingProvider tunnelingProvider, IConfiguration config) 
    : HandlerBase<GetHubByIdQuery, OperationResult<Hub>>(tunnelingRepository, config)
{
   public override async ValueTask<OperationResult<Hub>> Handle(GetHubByIdQuery request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);
        
        var deviceGroup = await tunnelingRepository.RetrieveDeviceGroupAsync(request.OrganizationId, request.Id, cancellationToken) 
            ?? throw new DataNotFoundException($"Information not found for hub {request.Id}.");

        var serverInformation = await tunnelingProvider.GetServerInformationAsync(request.OrganizationId, request.Id.ToString(), cancellationToken) 
            ?? throw new DataNotFoundException($"Hub {request.Id} not found.");

        return new OperationResult<Hub>(new Hub(deviceGroup, serverInformation.ConnectedDevices), true, Constants.StatusSuccess, []);
    }
}