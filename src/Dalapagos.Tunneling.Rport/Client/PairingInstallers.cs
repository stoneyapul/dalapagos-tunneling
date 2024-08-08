namespace Dalapagos.Tunneling.Rport.Client;

using System.Text.Json.Serialization;

public  class PairingInstallers
{
   [JsonPropertyName("linux")]
    public string Linux { get; set; } = default!;

   [JsonPropertyName("windows")]
    public string Windows { get; set; } = default!;

}
