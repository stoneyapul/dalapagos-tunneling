namespace Dalapagos.Tunneling.Api.Mappers;

using Core.Model;
using Dto;

public class HubsMapper : MapperBase<IList<Hub>, IList<HubResponse>>
{
    public override IList<HubResponse> Map(IList<Hub> source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        var responses = new List<HubResponse>();

        foreach (var hub in source)
        {
            ArgumentNullException.ThrowIfNull(hub.Id, nameof(hub.Id));

            responses.Add(
                new HubResponse
                {
                    TotalDevices = hub.TotalDeviceCount.HasValue && hub.TotalDeviceCount.Value > 0 ? hub.TotalDeviceCount : null,                    
                    HubId = hub.Id.Value,
                    Name = hub.Name,
                    Location = hub.Location.ToString(), 
                    Status = hub.Status.ToString()
                });
        }

        return responses;
    }
}
