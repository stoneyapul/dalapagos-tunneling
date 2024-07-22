namespace Dalapagos.Tunneling.Rport.Client;

using System.Text.Json.Serialization;

public class RportClientAuthData
{
    [JsonPropertyName("id")]
    public string ClientAuthId { get; set; } = default!;

    [JsonPropertyName("password")]
    public string Password { get; set; } = default!;
}
