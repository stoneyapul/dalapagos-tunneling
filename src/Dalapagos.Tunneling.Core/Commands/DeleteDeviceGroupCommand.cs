namespace Dalapagos.Tunneling.Core.Commands;

using System.Threading;
using System.Threading.Tasks;
using Azure.ResourceManager;
using Azure.Security.KeyVault.Secrets;
using Infrastructure;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;

public record DeleteDeviceGroupCommand(Guid Id, Guid OganizationId) : IRequest<OperationResult>;

public class DeleteDeviceGroupHandler(ILogger<DeleteDeviceGroupCommand> logger, IConfiguration config, ITunnelingRepository tunnelingRepository) 
    : CommandBase, IRequestHandler<DeleteDeviceGroupCommand, OperationResult>
{
    public async ValueTask<OperationResult> Handle(DeleteDeviceGroupCommand request, CancellationToken cancellationToken)
    {
        var keyVaultName = config["KeyVaultName"]!;
        var shortDeviceGrpId = request.Id.ToString().Substring(24, 12).ToLowerInvariant();
        var deviceGroup = await tunnelingRepository.RetrieveDeviceGroupAsync(request.OganizationId, request.Id, cancellationToken);

        // Delete the resource group that has the VM. This takes awhile, so we will continue on without waiting for it to finish.
        var resourceGroupName = $"dlpg-{shortDeviceGrpId}";
        logger.LogInformation("Deleting resource group {ResourceGroup}.", resourceGroupName);
        var armClient = new ArmClient(GetTokenCredential(config));
        var subscription = await armClient.GetDefaultSubscriptionAsync(cancellationToken);
        var resourceGroup = await subscription.GetResourceGroupAsync(resourceGroupName, cancellationToken);
        await resourceGroup.Value.DeleteAsync(Azure.WaitUntil.Started, cancellationToken: cancellationToken);

        // Delete key vault secrets.
        logger.LogInformation("Deleting secrets for {DeviceGrpId}.", shortDeviceGrpId);
        var credential = GetTokenCredential(config);
        var keyVaultUrl = "https://" + keyVaultName + ".vault.azure.net";
        var keyVaultClient = new SecretClient(new Uri(keyVaultUrl), credential);
        await keyVaultClient.StartDeleteSecretAsync($"{shortDeviceGrpId}-Tnls-Finger", cancellationToken);
        await keyVaultClient.StartDeleteSecretAsync($"{shortDeviceGrpId}-Tnls-RPortPass", cancellationToken);
        await keyVaultClient.StartDeleteSecretAsync($"{shortDeviceGrpId}-Tnls-VmPass", cancellationToken);

        // Any devices in the group should be orphaned, ie no longer associated with a group.
        if (deviceGroup.Devices != null)
        {
            foreach (var device in deviceGroup.Devices)
            {
                await tunnelingRepository.UpsertDeviceAsync(device.Id, null, device.Name, cancellationToken);
            }
        }

        await tunnelingRepository.DeleteDeviceGroupAsync(request.Id, cancellationToken);           
        return new OperationResult(true, []);
    }
}