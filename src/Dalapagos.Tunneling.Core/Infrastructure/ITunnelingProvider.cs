namespace Dalapagos.Tunneling.Core.Infrastructure;

using Model;

public interface ITunnelingProvider
{
    Task<TunnelServer> GetServerInformationAsync(
        Guid organizationId, 
        Guid deviceGroupId, 
        CancellationToken cancellationToken = default);

    Task<IList<Tunnel>> GetTunnelsByDeviceIdAsync(
        Guid organizationId,
        Guid deviceId,
        CancellationToken cancellationToken = default
    );

    Task<bool> IsDeviceConnectedAsync(
        Guid organizationId,
        Guid deviceId,
        CancellationToken cancellationToken = default
    );

    Task<Tunnel> AddTunnelAsync(
        Guid organizationId,
        Guid deviceId,
        Protocol protocol,
        ushort port,
        int? deleteAfterMin,
        string? allowedIp = null,
        CancellationToken cancellationToken = default
    );

    Task RemoveTunnelAsync(
        Guid organizationId,
        Guid deviceId,
        string tunnelId,
        CancellationToken cancellationToken = default
    );

    Task RemoveTunnelCredentialsAsync(
        Guid organizationId,
        Guid deviceId, 
        CancellationToken cancellationToken = default);
}