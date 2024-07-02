namespace Dalapagos.Tunneling.Core.Model;

public record class DeviceGroup(
    Guid? Id, 
    Guid OrganizationId, 
    string Name, 
    ServerLocation ServerLocation, 
    ServerStatus ServerStatus, 
    IList<Device>? Devices = null);
