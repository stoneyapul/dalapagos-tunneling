namespace Dalapagos.Tunneling.Core.Model;

public record DeviceGroup(
    Guid? Id, 
    Guid OrganizationId, 
    string Name, 
    ServerLocation ServerLocation, 
    ServerStatus ServerStatus, 
    string? ServerBaseUrl,
    IList<Device>? Devices = null)
    {
        public ServerStatus ServerStatus { get; internal set; } = ServerStatus;
    }
