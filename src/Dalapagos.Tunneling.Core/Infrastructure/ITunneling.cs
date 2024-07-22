namespace Dalapagos.Tunneling.Core.Infrastructure;

using Model;

public interface ITunneling
{
    Task<TunnelServer> GetServerInformationAsync(CancellationToken cancellationToken = default);

    Task<IList<Tunnel>> GetTunnelsByDeviceIdAsync(
        Guid deviceId,
        CancellationToken cancellationToken = default
    );

    Task<bool> IsDeviceConnectedAsync(
        string deviceId,
        CancellationToken cancellationToken = default
    );

    Task<Tunnel> AddTunnelAsync(
        Guid deviceId,
        Protocol protocol,
        ushort port,
        int? deleteAfterMin,
        string? allowedIp = null,
        CancellationToken cancellationToken = default
    );

    Task RemoveTunnelAsync(
        Guid deviceId,
        string tunnelId,
        CancellationToken cancellationToken = default
    );

    Task RemoveTunnelCredentialsAsync(
        Guid deviceId, 
        CancellationToken cancellationToken = default);
}