namespace Dalapagos.Tunneling.Core.Infrastructure;

using Model;

public interface ITunnelingRepository
{
    Task<Organization> UpsertOrganizationAsync(Guid? organizationId, string organizationName, CancellationToken cancellationToken);

    Task<Organization> RetrieveOrganizationAsync(Guid organizationId, CancellationToken cancellationToken); 

    Task<IList<Organization>> GetOrganizationsAsync(CancellationToken cancellationToken);

    Task DeleteOrganizationAsync(Guid organizationId, CancellationToken cancellationToken);

    Task<Device> UpsertDeviceAsync(Guid? deviceId, Guid? deviceGroupId, string deviceName, CancellationToken cancellationToken);

    Task<Device> RetrieveDeviceAsync(Guid deviceGroupId, Guid deviceId, CancellationToken cancellationToken);

    Task DeleteDeviceAsync(Guid deviceId, CancellationToken cancellationToken);
    
    Task<DeviceGroup> UpsertDeviceGroupAsync(
        Guid? deviceGroupId, 
        Guid organizationId, 
        string deviceGroupName, 
        string serverName, 
        string serverLocation, 
        ServerStatus serverStatus, 
        Guid? adminGroupId, 
        Guid? userGroupId, 
        CancellationToken cancellationToken);

    Task<DeviceGroup> RetrieveDeviceGroupAsync(Guid organizationId, Guid deviceGroupId, CancellationToken cancellationToken);

    Task DeleteDeviceGroupAsync(Guid deviceGroupId, CancellationToken cancellationToken);
}
