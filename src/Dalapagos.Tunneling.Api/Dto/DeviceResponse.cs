namespace Dalapagos.Tunneling.Api.Dto;

public class DeviceResponse
{
    public Guid DeviceId { get; set; }

    public Guid? DeviceGroupId { get; set; }

    public string Name { get; set; } = default!;

    public string Os { get; set; } = default!;
}
