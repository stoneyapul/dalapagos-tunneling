namespace Dalapagos.Tunneling.Rport.Client;

using System.Text.Json.Serialization;

public class RportTunnel
{
    [JsonPropertyName("data")]
    public RportTunnelData Data { get; set; } = default!;
}
