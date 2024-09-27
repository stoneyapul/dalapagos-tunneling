namespace Dalapagos.Tunneling.Api.Dto;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a response with tunnel information. 
/// </summary>
public class TunnelResponse
{
    /// <summary>
    /// The tunnel protocol. Https or SSH.
    /// </summary>
    [JsonPropertyName("protocol")]
    public required string Protocol { get; set; }

    /// <summary>
    /// The tunnel URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

   /// <summary>
    /// The IP addresses that can use the tunnel.
    /// </summary>
    [JsonPropertyName("allowed-ips")]
    public string[]? AllowedIps { get; set; }
}
