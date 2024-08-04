namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Commands;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Model;

internal sealed class AddDeviceHandler(ITunnelingRepository tunnelingRepository, ITunnelingProvider tunnelingProvider, IConfiguration config) 
    : HandlerBase<AddDeviceCommand, OperationResult<Device>>(tunnelingRepository, config)
{
    public override async ValueTask<OperationResult<Device>> Handle(AddDeviceCommand request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);

        var device = await tunnelingRepository.UpsertDeviceAsync(
            request.Id.HasValue ? request.Id : Guid.NewGuid(), 
            request.HubId,
            request.Name,
            request.Os, 
            request.OrganizationId,
            cancellationToken);
            
    //     try
    //     {
    //         await tunnelingProvider.AddDeviceCredentialStringAsync(request.HubId, device.Id, cancellationToken);
    //         return creds.Data.Password;
    //     }
    //     catch (Refit.ApiException ex)
    //     {
    //         if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
    //         {
    //             return await AddClientAuthAsync(tunnelingDeviceIdNotNull, cancellationToken);
    //         }

    //         logger.Error("Message: {message} {content}", ex.RequestMessage, ex.Content);
    //         throw new TunnelingException(GetErrorMessage(ex), ex.StatusCode);
    //     }

    //    await tunnelingProvider.
        return new OperationResult<Device>(device, true, Constants.StatusSuccessCreated, []);
    }
}