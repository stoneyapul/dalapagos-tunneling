namespace Dalapagos.Tunneling.Rport.Client;

using System.Text.Json.Serialization;

public class RportClientData
{
    [JsonPropertyName("id")]
    public string? ClientId { get; set; }

    [JsonPropertyName("name")]
    public string? ClientName { get; set; }

    [JsonPropertyName("hostname")]
    public string? Hostname { get; set; }

    [JsonPropertyName("os_full_name")]
    public string? OsFullName { get; set; }

    [JsonPropertyName("cpu_model_name")]
    public string? CpuName { get; set; }

    [JsonPropertyName("num_cpus")]
    public int? Cpus { get; set; }

    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }

    [JsonPropertyName("ipv4")]
    public string[]? Ips { get; set; }

    [JsonPropertyName("connection_state")]
    public string? ConnectionState { get; set; }

    [JsonPropertyName("tunnels")]
    public RportTunnelData[] Tunnels { get; set; } = [];
}
