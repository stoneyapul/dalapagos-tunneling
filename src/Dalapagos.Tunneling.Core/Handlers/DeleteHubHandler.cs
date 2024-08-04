namespace Dalapagos.Tunneling.Core.Handlers;

using System.Threading;
using System.Threading.Tasks;
using Azure.ResourceManager;
using Commands;
using Extensions;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;

internal sealed  class DeleteHubHandler(
    ILogger<DeleteHubCommand> logger, 
    IConfiguration config, 
    ISecrets secrets,
    ITunnelingRepository tunnelingRepository)
    : HandlerBase<DeleteHubCommand, OperationResult>(tunnelingRepository, config)
{
   public override async ValueTask<OperationResult> Handle(DeleteHubCommand request, CancellationToken cancellationToken)
    {
        await VerifyUserOrganizationAsync(request, cancellationToken);
        
        var deviceGroup = await tunnelingRepository.RetrieveDeviceGroupAsync(request.OrganizationId, request.Id, cancellationToken);
        if (deviceGroup.ServerStatus == ServerStatus.Deploying)
        {
            throw new Exception($"Hub {request.Id.ToShortHubId()} is deploying. It cannot be deleted until the deployment is complete.");
        }

        var warnings = new List<string>();   

        // Delete the resource group that has the VM. This takes awhile, so we will continue on without waiting for it to finish.
        var resourceGroupName = $"dlpg-{request.Id.ToShortHubId()}";
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
            logger.LogWarning(aex, "Failed to delete resource group for {DeviceGrpId}.", request.Id.ToShortHubId());
        }

        // Delete key vault secrets.
        logger.LogInformation("Deleting secrets for {DeviceGrpId}.", request.Id.ToShortHubId());

        try
        {
            await secrets.RemoveSecretAsync($"{request.Id.ToShortHubId()}{Constants.TunnelingServerFingerprintNameSfx}", cancellationToken);
        }
        catch (Azure.RequestFailedException aex)
        {
            if (aex.ErrorCode == null || !aex.ErrorCode.Equals("SecretNotFound", StringComparison.OrdinalIgnoreCase))
            {
                throw;
            }

            warnings.Add("Failed to delete the tunneling server fingerprint.");
            logger.LogWarning(
                aex, 
                "Failed to delete secret {Secret} for {DeviceGrpId}.", 
                $"{request.Id.ToShortHubId()}{Constants.TunnelingServerFingerprintNameSfx}", request.Id.ToShortHubId());
        }

        try
        {
            await secrets.RemoveSecretAsync($"{request.Id.ToShortHubId()}{Constants.TunnelingServerPassNameSfx}", cancellationToken);
        }
        catch (Azure.RequestFailedException aex)
        {
            if (aex.ErrorCode == null || !aex.ErrorCode.Equals("SecretNotFound", StringComparison.OrdinalIgnoreCase))
            {
                throw;
            }

            warnings.Add("Failed to delete the tunneling server password.");
            logger.LogWarning(
                aex, 
                "Failed to delete secret {Secret} for {DeviceGrpId}.", 
                $"{request.Id.ToShortHubId()}{Constants.TunnelingServerPassNameSfx}", request.Id.ToShortHubId());
        }

        try
        {
            await secrets.RemoveSecretAsync($"{request.Id.ToShortHubId()}{Constants.TunnelingServerVmPassNameSfx}", cancellationToken);
        }
        catch (Azure.RequestFailedException aex)
        {
            if (aex.ErrorCode == null || !aex.ErrorCode.Equals("SecretNotFound", StringComparison.OrdinalIgnoreCase))
            {
                throw;
            }

            warnings.Add("Failed to delete the virtual machine password.");
            logger.LogWarning(
                aex, 
                "Failed to delete secret {Secret} for {DeviceGrpId}.", 
                $"{request.Id.ToShortHubId()}{Constants.TunnelingServerVmPassNameSfx}", 
                request.Id.ToShortHubId());
        }

        // Any devices in the group should be orphaned, ie no longer associated with a group.
        if (deviceGroup.Devices != null)
        {
            foreach (var device in deviceGroup.Devices)
            {
                await tunnelingRepository.UpsertDeviceAsync(
                    device.Id, 
                    null, 
                    device.Name, 
                    device.Os, 
                    request.OrganizationId, 
                    cancellationToken);
            }
        }

        await tunnelingRepository.DeleteDeviceGroupAsync(request.Id, cancellationToken);           
        return new OperationResult(true, Constants.StatusSuccess, warnings.Count != 0 ? [.. warnings] : []);
    }
}