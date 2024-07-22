namespace Dalapagos.Tunneling.Rport.Client;

using System.Text.Json.Serialization;

public class RportErrors
{
    [JsonPropertyName("errors")]
    public RportError[] Errors { get; set; } = default!;
}
