namespace Dalapagos.Tunneling.Api.Mappers;

using Core.Model;
using Dto;

public class DeviceMapper : MapperBase<Device, DeviceResponse>
{
    public override DeviceResponse Map(Device source)
    {
        ArgumentNullException.ThrowIfNull(source.Id, nameof(source.Id));

        return new DeviceResponse
        {
            DeviceId = source.Id.Value,
            HubId = source.HubId,
            Name = source.Name,
            Os = source.Os.ToString()
        };
    }
}
