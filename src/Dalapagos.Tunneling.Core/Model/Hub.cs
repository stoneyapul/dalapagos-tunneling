namespace Dalapagos.Tunneling.Core.Model;

public record Hub(DeviceGroup DeviceGroup, int? ConnectedDeviceCount = null)
{
    public int? ConnectedDeviceCount { get; init; } = ConnectedDeviceCount;

    public int? TotalDeviceCount { get; init; } = DeviceGroup.Devices?.Count;

    public Guid? Id { get; init; } = DeviceGroup.Id;

    public Guid OrganizationId { get; init; } = DeviceGroup.OrganizationId;

    public string Name { get; init; } = DeviceGroup.Name;

    public ServerLocation Location { get; init; } = DeviceGroup.ServerLocation;
    
    public ServerStatus Status { get; init; } = DeviceGroup.ServerStatus;

    public string? BaseUrl { get; init; } = DeviceGroup.ServerBaseUrl;
 
    public IList<Device>? Devices
    { 
        get 
        { 
            return DeviceGroup.Devices; 
        } 
    }
}
