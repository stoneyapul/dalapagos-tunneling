namespace Dalapagos.Tunneling.Core.Model;

public record Tunnel(string? TunnelId, ushort DevicePort, ushort TunnelPort)
{
    public Protocol Protocol { get; set; }

    public string? Url { get; set; }

    public string[]? AllowedIps { get; set; }
}
