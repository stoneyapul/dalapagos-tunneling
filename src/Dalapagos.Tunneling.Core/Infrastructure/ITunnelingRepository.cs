namespace Dalapagos.Tunneling.Core.Infrastructure;

using Model;

public interface ITunnelingRepository
{
    Task<Organization> UpsertOrganizationAsync(Guid? organizationId, string organizationName, CancellationToken cancellationToken);

    Task<Organization> RetrieveOrganizationAsync(Guid organizationId, CancellationToken cancellationToken); 

    Task<IList<Organization>> GetOrganizationsAsync(CancellationToken cancellationToken);

    Task<IList<OrganizationUser>> GetOrganizationUsersByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task DeleteOrganizationAsync(Guid organizationId, CancellationToken cancellationToken);

    Task<Device> UpsertDeviceAsync(Guid? deviceId, Guid? deviceGroupId, string deviceName, Os os, CancellationToken cancellationToken);

    Task DeleteDeviceAsync(Guid deviceId, CancellationToken cancellationToken);
    
    Task<DeviceGroup> UpsertDeviceGroupAsync(
        Guid? deviceGroupId, 
        Guid organizationId, 
        string deviceGroupName, 
        ServerLocation serverLocation, 
        ServerStatus serverStatus, 
        string? serverBaseUrl,
        CancellationToken cancellationToken);

    Task UpdateDeviceGroupServerStatusAsync(Guid deviceGroupId, ServerStatus serverStatus, CancellationToken cancellationToken);

    Task<DeviceGroup> RetrieveDeviceGroupAsync(Guid organizationId, Guid deviceGroupId, CancellationToken cancellationToken);

    Task DeleteDeviceGroupAsync(Guid deviceGroupId, CancellationToken cancellationToken);

    Task<string> RetrieveServerBaseAddressAsync(Guid deviceGroupId, CancellationToken cancellationToken);
}
