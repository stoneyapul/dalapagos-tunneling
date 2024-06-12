namespace Dalapagos.Tunneling.Core.Model;

public record class DeviceGroup(
    Guid? DeviceGroupId, 
    Guid OrganizationId, 
    string DeviceGroupName, 
    string ServerName, 
    string ServerLocation, 
    ServerStatus ServerStatus, 
    Guid? AdminGroupId = null, 
    Guid? UserGroupId = null,
    IList<Device>? Devices = null);
