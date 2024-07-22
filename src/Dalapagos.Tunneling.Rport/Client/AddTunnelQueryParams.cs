namespace Dalapagos.Tunneling.Rport.Client;

using Refit;

public class AddTunnelQueryParams(
    string remotePort,
    string scheme,
    int? deleteAfterMin,
    string[]? allowedIps)
{
    [AliasAs("remote")]
    public string RemotePort { get; } = remotePort;

    [AliasAs("scheme")]
    public string Scheme { get; } = scheme;

    [AliasAs("auto-close")]
    public string? DeleteAfterMin { get; } = $"{deleteAfterMin}m";

    [AliasAs("acl")]
    public string[]? AllowedIps { get; } = allowedIps;
}
