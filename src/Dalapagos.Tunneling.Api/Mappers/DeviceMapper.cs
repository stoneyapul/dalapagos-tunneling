namespace Dalapagos.Tunneling.Api.Mappers;

using Core.Model;
using Dto;

public class DeviceMapper : MapperBase<Device, DeviceResponse>
{
    public override DeviceResponse Map(Device device)
    {
        ArgumentNullException.ThrowIfNull(device.Id, nameof(device.Id));

        return new DeviceResponse
        {
            DeviceId = device.Id.Value,
            DeviceGroupId = device.DeviceGroupId,
            Name = device.Name,
            Os = device.Os.ToString()
        };
    }
}
