namespace Dalapagos.Tunneling.Rport.Client;

using System.Text.Json.Serialization;

public class PairingRequest
{
    [JsonPropertyName("client_id")]
    public string ClientAuthId { get; set; } = default!;

    [JsonPropertyName("password")]
    public string Password { get; set; } = default!;

   [JsonPropertyName("connect_url")]
    public string ConnectUrl { get; set; } = default!;

   [JsonPropertyName("fingerprint")]
    public string Fingerprint { get; set; } = default!;

}