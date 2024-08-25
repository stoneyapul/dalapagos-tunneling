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

    Task<string?> ConfigureDeviceConnectionAsync(
        Guid hubId,
        Guid deviceId, 
        string serverBaseAddress,
        string clientCredentialString,
        Os os,
        CancellationToken cancellationToken = default);

    Task<string> GetPairingScriptAsync(
        Guid hubId,
        Guid deviceId, 
        string serverBaseAddress,
        Os os,
        CancellationToken cancellationToken = default);

    Task RemoveDeviceCredentialsAsync(
        Guid hubId,
        Guid deviceId, 
        string baseAddress,
        CancellationToken cancellationToken = default);
}