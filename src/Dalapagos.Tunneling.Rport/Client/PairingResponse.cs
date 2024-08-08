namespace Dalapagos.Tunneling.Rport.Client;

using System.Text.Json.Serialization;

public class PairingResponse
{
    [JsonPropertyName("pairing_code")]
    public required string PairingCode { get; set; }

    [JsonPropertyName("expires")]
    public DateTime? Expires { get; set; }

   [JsonPropertyName("installers")]
    public required PairingInstallers Installers { get; set; }
}
