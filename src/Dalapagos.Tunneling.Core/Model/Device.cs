namespace Dalapagos.Tunneling.Core.Model;

public record Device(Guid? Id, Guid? HubId, string Name, Os Os, RestProtocol? RestProtocol, ushort? RestPort)
{
    public string? PairingScript { get; set; }
}
