namespace Dalapagos.Tunneling.Rport.Client;

using System.Text.Json.Serialization;

public class RportClientData
{
    [JsonPropertyName("id")]
    public string? ClientId { get; set; }

    [JsonPropertyName("name")]
    public string? ClientName { get; set; }

    [JsonPropertyName("connection_state")]
    public string? ConnectionState { get; set; }

    [JsonPropertyName("tunnels")]
    public RportTunnelData[] Tunnels { get; set; } = default!;
}
