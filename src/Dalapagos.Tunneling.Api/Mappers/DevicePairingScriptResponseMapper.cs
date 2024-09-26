namespace Dalapagos.Tunneling.Api.Mappers;

using Dto;

public class DevicePairingScriptResponseMapper : MapperBase<string?, DevicePairingScriptResponse>
{
    public override DevicePairingScriptResponse Map(string? source)
    {
        return new DevicePairingScriptResponse{ PairingScript = source };
    }
}
