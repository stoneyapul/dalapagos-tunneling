namespace Dalapagos.Tunneling.Rport.Client;

using System.Text.Json.Serialization;

public class RportClient
{
    [JsonPropertyName("data")]
    public RportClientData Data { get; set; } = default!;
}
