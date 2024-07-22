namespace Dalapagos.Tunneling.Rport.Client;

using System.Text.Json.Serialization;

public class RportServer
{
    [JsonPropertyName("data")]
    public RportServerData Data { get; set; } = default!;
}
