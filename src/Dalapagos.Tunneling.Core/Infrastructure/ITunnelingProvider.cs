namespace Dalapagos.Tunneling.Core.Infrastructure;

using Model;

public interface ITunnelingProvider
{
    Task<TunnelServer> GetServerInformationAsync(
        Guid hubId,
        string baseAddress,
        CancellationToken cancellationToken = default);

    Task<IList<Tunnel>> GetTunnelsByDeviceIdAsync(
        Guid hubId,
        Guid deviceId, 
        string baseAddress,
        CancellationToken cancellationToken = default
    );

    Task<bool> IsDeviceConnectedAsync(
        Guid hubId,
        Guid deviceId, 
        string baseAddress,
        CancellationToken cancellationToken = default
    );

    Task<Tunnel> AddTunnelAsync(
        Guid hubId,
        Guid deviceId, 
        string baseAddress,
        Protocol protocol,
        ushort port,
        int? deleteAfterMin,
        string? allowedIp = null,
        CancellationToken cancellationToken = default
    );

    Task RemoveTunnelAsync(
        Guid hubId,
        Guid deviceId, 
        string baseAddress,
        string tunnelId,
        CancellationToken cancellationToken = default
    );

    Task<string> GetDeviceCredentialStringAsync(
        Guid hubId,
        Guid deviceId, 
        string baseAddress,
        CancellationToken cancellationToken = default);

    Task AddDeviceCredentialStringAsync(
        Guid hubId,
        Guid deviceId, 
        string baseAddress,
        string credentialString,
        CancellationToken cancellationToken = default);

    Task RemoveDeviceCredentialStringAsync(
        Guid hubId,
        Guid deviceId, 
        string baseAddress,
        CancellationToken cancellationToken = default);
}