namespace Dalapagos.Tunneling.Api.Dto;

public class DeviceGroupResponse
{
    public Guid? DeviceGroupId { get; set; }

    public string Name { get; set; } = default!;

    public string Location { get; set; } = default!;

    public string Status { get; set; } = default!;

    public IList<DeviceResponse>? Devices { get; set; }
}
