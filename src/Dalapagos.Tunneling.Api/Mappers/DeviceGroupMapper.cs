namespace Dalapagos.Tunneling.Api.Mappers;

using Core.Model;
using Dto;

public class DeviceGroupMapper : MapperBase<DeviceGroup, DeviceGroupResponse>
{
    public override DeviceGroupResponse Map(DeviceGroup source)
    {
        ArgumentNullException.ThrowIfNull(source.Id, nameof(source.Id));

        var deviceMapper = new DeviceMapper();
        var devices = new List<DeviceResponse>();

        if (source.Devices != null)
        {
            foreach (var device in source.Devices)
            {
                devices.Add(deviceMapper.Map(device));
            }
        }

        return new DeviceGroupResponse
        {
            DeviceGroupId = source.Id.Value,
            Name = source.Name,
            Location = source.ServerLocation.ToString(), 
            Status = source.ServerStatus.ToString(), 
            Devices = devices
        };
    }
}