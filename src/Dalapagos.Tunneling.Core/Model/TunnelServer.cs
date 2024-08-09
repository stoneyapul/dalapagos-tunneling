namespace Dalapagos.Tunneling.Core.Model;

public record TunnelServer(
    ServerStatus Status,
    string? ErrorMessage,
    string? Version,
    string? Fingerprint,
    int? ConnectedDevices
);
