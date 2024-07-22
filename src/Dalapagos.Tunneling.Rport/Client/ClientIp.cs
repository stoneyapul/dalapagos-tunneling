namespace Dalapagos.Tunneling.Rport.Client;

using System.Text.Json.Serialization;

public class ClientIp
{
    [JsonPropertyName("data")]
    public ClientIpData Data { get; set; } = default!;
}
