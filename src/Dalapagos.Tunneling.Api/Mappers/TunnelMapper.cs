namespace Dalapagos.Tunneling.Api.Mappers;

using Core.Model;
using Dto;

public class TunnelMapper : MapperBase<Tunnel, TunnelResponse>
{
    public override TunnelResponse Map(Tunnel source)
    {
        return new TunnelResponse
        {
            Protocol = source.Protocol.ToString(),
            AllowedIps    = source.AllowedIps,
            Url = source.Url
        };
    }
}
