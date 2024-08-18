namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Model;
using Queries;

internal sealed class GetHubsByOrganizationIdHandler(ITunnelingRepository tunnelingRepository, IConfiguration config) 
    : HandlerBase<GetHubsByOrganizationIdQuery, OperationResult<IList<Hub>>>(tunnelingRepository, config)
{
   public override async ValueTask<OperationResult<IList<Hub>>> Handle(GetHubsByOrganizationIdQuery request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);
        
        var organization = await tunnelingRepository.RetrieveOrganizationAsync(request.OrganizationId, cancellationToken)
            ?? throw new DataNotFoundException($"Organization {request.OrganizationId} not found.");

        if (organization.DeviceGroups == null || !organization.DeviceGroups.Any())
        {
            return new OperationResult<IList<Hub>>([], true, Constants.StatusSuccess, []);
        }

        var hubs = new List<Hub>();
        foreach (var deviceGroup in organization.DeviceGroups)
        {
            hubs.Add(new Hub(deviceGroup, null));
        }

        return new OperationResult<IList<Hub>>(hubs, true, Constants.StatusSuccess, []);
    }
}