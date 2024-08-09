namespace Dalapagos.Tunneling.Rport.Client;

using System.Text.Json.Serialization;

public class RportServerData
{
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("fingerprint")]
    public string? Fingerprint { get; set; }

    [JsonPropertyName("clients_connected")]
    public int? ClientsConnected { get; set; }

    [JsonPropertyName("clients_disconnected")]
    public int? ClientsDisconnected { get; set; }

    [JsonPropertyName("used_ports")]
    public string[]? UsedPorts { get; set; }
}
