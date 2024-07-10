namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Azure.ResourceManager;
using Azure.Security.KeyVault.Secrets;
using Commands;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;

internal sealed  class DeleteDeviceGroupHandler(ILogger<DeleteDeviceGroupCommand> logger, IConfiguration config, ITunnelingRepository tunnelingRepository)
    : HandlerBase<DeleteDeviceGroupCommand, OperationResult>
{
   public override async ValueTask<OperationResult> Handle(DeleteDeviceGroupCommand request, CancellationToken cancellationToken)
    {
        var keyVaultName = config["KeyVaultName"]!;
        var shortDeviceGrpId = request.Id.ToString().Substring(24, 12).ToLowerInvariant();
        var deviceGroup = await tunnelingRepository.RetrieveDeviceGroupAsync(request.OrganizationId, request.Id, cancellationToken);
        var warnings = new List<string>();   

        // Delete the resource group that has the VM. This takes awhile, so we will continue on without waiting for it to finish.
        var resourceGroupName = $"dlpg-{shortDeviceGrpId}";
        logger.LogInformation("Deleting resource group {ResourceGroup}.", resourceGroupName);
        try
        {
            var armClient = new ArmClient(GetTokenCredential(config));
            var subscription = await armClient.GetDefaultSubscriptionAsync(cancellationToken);
            var resourceGroup = await subscription.GetResourceGroupAsync(resourceGroupName, cancellationToken);
            await resourceGroup.Value.DeleteAsync(Azure.WaitUntil.Started, cancellationToken: cancellationToken);
        }
        catch (Azure.RequestFailedException aex)
        {
            if (aex.ErrorCode == null || !aex.ErrorCode.Equals("ResourceGroupNotFound", StringComparison.OrdinalIgnoreCase))
            {
                throw;
            }
            warnings.Add("Failed to delete the resource group.");
            logger.LogWarning(aex, "Failed to delete resource group for {DeviceGrpId}.", shortDeviceGrpId);
        }

        // Delete key vault secrets.
        logger.LogInformation("Deleting secrets for {DeviceGrpId}.", shortDeviceGrpId);
        var credential = GetTokenCredential(config);
        var keyVaultUrl = "https://" + keyVaultName + ".vault.azure.net";
        var keyVaultClient = new SecretClient(new Uri(keyVaultUrl), credential);

        try
        {
            await keyVaultClient.StartDeleteSecretAsync($"{shortDeviceGrpId}-Tnls-Finger", cancellationToken);
        }
        catch (Azure.RequestFailedException aex)
        {
            if (aex.ErrorCode == null || !aex.ErrorCode.Equals("SecretNotFound", StringComparison.OrdinalIgnoreCase))
            {
                throw;
            }
            warnings.Add("Failed to delete the tunneling server fingerprint.");
            logger.LogWarning(aex, "Failed to delete secret {Secret} for {DeviceGrpId}.", $"{shortDeviceGrpId}-Tnls-Finger", shortDeviceGrpId);
        }

        try
        {
            await keyVaultClient.StartDeleteSecretAsync($"{shortDeviceGrpId}-Tnls-RPortPass", cancellationToken);
        }
        catch (Azure.RequestFailedException aex)
        {
            if (aex.ErrorCode == null || !aex.ErrorCode.Equals("SecretNotFound", StringComparison.OrdinalIgnoreCase))
            {
                throw;
            }
            warnings.Add("Failed to delete the tunneling server password.");
            logger.LogWarning(aex, "Failed to delete secret {Secret} for {DeviceGrpId}.", $"{shortDeviceGrpId}-Tnls-RPortPass", shortDeviceGrpId);
        }

        try
        {
           await keyVaultClient.StartDeleteSecretAsync($"{shortDeviceGrpId}-Tnls-VmPass", cancellationToken);
        }
        catch (Azure.RequestFailedException aex)
        {
            if (aex.ErrorCode == null || !aex.ErrorCode.Equals("SecretNotFound", StringComparison.OrdinalIgnoreCase))
            {
                throw;
            }
            warnings.Add("Failed to delete the virtual machine password.");
            logger.LogWarning(aex, "Failed to delete secret {Secret} for {DeviceGrpId}.", $"{shortDeviceGrpId}-Tnls-VmPass", shortDeviceGrpId);
        }

        // Any devices in the group should be orphaned, ie no longer associated with a group.
        if (deviceGroup.Devices != null)
        {
            foreach (var device in deviceGroup.Devices)
            {
                await tunnelingRepository.UpsertDeviceAsync(device.Id, null, device.Name, device.Os, cancellationToken);
            }
        }

        await tunnelingRepository.DeleteDeviceGroupAsync(request.Id, cancellationToken);           
        return new OperationResult(true, Constants.StatusSuccess, warnings.Count != 0 ? [.. warnings] : []);
    }
}
