namespace Dalapagos.Tunneling.Rport.Client;

using System.Text.Json.Serialization;

public class RportError
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("detail")]
    public string? Detail { get; set; }
}
