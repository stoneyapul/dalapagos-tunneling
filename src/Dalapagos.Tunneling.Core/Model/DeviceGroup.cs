namespace Dalapagos.Tunneling.Core.Model;

public record class DeviceGroup(
    Guid? Id, 
    Guid OrganizationId, 
    string Name, 
    string ServerName, 
    string ServerLocation, 
    ServerStatus ServerStatus, 
    Guid? AdminGroupId = null, 
    Guid? UserGroupId = null,
    IList<Device>? Devices = null);
