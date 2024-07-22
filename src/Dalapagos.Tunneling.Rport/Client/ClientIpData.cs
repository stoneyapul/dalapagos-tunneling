namespace Dalapagos.Tunneling.Rport.Client;

using System.Text.Json.Serialization;

public class ClientIpData
{
    [JsonPropertyName("ip")]
    public string Ip { get; set; } = default!;
}
