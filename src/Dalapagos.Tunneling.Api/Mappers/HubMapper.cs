namespace Dalapagos.Tunneling.Api.Mappers;

using Core.Model;
using Dto;

public class HubMapper : MapperBase<Hub, HubResponse>
{
    public override HubResponse Map(Hub source)
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

        return new HubResponse
        {
            ConnectedDevices = source.ConnectedDeviceCount,
            TotalDevices = source.TotalDeviceCount.HasValue && source.TotalDeviceCount.Value > 0 ? source.TotalDeviceCount : null,
            HubId = source.Id.Value,
            Name = source.Name,
            Location = source.Location.ToString(), 
            Status = source.Status.ToString(), 
            Devices = devices
        };
    }
}