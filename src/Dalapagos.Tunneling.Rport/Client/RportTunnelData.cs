namespace Dalapagos.Tunneling.Rport.Client;

using System.Text.Json.Serialization;

public class RportTunnelData
{
    [JsonPropertyName("id")]
    public string? TunnelId { get; set; }

    [JsonPropertyName("client_id")]
    public string? ClientId { get; set; }

    [JsonPropertyName("lhost")]
    public string? LocalHost { get; set; }

    [JsonPropertyName("lport")]
    public string? LocalPort { get; set; }

    [JsonPropertyName("rhost")]
    public string? RemoteHost { get; set; }

    [JsonPropertyName("rport")]
    public string? RemotePort { get; set; }

    [JsonPropertyName("lport_random")]
    public bool? IsRandomLocalPort { get; set; }

    [JsonPropertyName("scheme")]
    public string? Scheme { get; set; }

    [JsonPropertyName("protocol")]
    public string? Protocol { get; set; }

    [JsonPropertyName("acl")]
    public string? Acl { get; set; }

    [JsonPropertyName("http_proxy")]
    public bool? HttpProxy { get; set; }

    [JsonPropertyName("host_header")]
    public string? HostHeader { get; set; }

    [JsonPropertyName("tunnel_url")]
    public string? TunnelUrl { get; set; }
}
