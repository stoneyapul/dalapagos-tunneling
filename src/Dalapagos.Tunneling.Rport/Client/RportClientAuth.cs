namespace Dalapagos.Tunneling.Rport.Client;

using System.Text.Json.Serialization;

public class RportClientAuth
{
    [JsonPropertyName("data")]
    public RportClientAuthData Data { get; set; } = default!;
}
