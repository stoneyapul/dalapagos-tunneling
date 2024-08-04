namespace Dalapagos.Tunneling.Rport.Client;

using Refit;

public interface IRportTunnelClient
{
    [Get("/api/v1/status")]
    Task<RportServer> GetServer(CancellationToken cancellationToken);

    [Get("/api/v1/clients/{client_id}")]
    Task<RportClient> GetClientById(
        [AliasAs("client_id")] string clientId, 
        CancellationToken cancellationToken);

    [Put("/api/v1/clients/{client_id}/tunnels")]
    Task<RportTunnel> AddClientTunnel(
        [AliasAs("client_id")] string clientId,
        [Query] AddTunnelQueryParams queryString,
        CancellationToken cancellationToken
    );

    [Delete("/api/v1/clients/{client_id}/tunnels/{tunnel_id}?force=true")]
    Task RemoveClientTunnel(
        [AliasAs("client_id")] string clientId,
        [AliasAs("tunnel_id")] string tunnelId,
        CancellationToken cancellationToken
    );

    [Get("/api/v1/clients-auth/{client_auth_id}")]
    Task<RportClientAuth> GetClientAuthById(
        [AliasAs("client_auth_id")] string clientAuthId,
        CancellationToken cancellationToken
    );

    [Post("/api/v1/clients-auth")]
    Task AddClientAuth(
        RportClientAuthData request,
        CancellationToken cancellationToken
    );

    [Delete("/api/v1/clients-auth/{client_auth_id}?force=true")]
    Task RemoveClientAuth(
        [AliasAs("client_auth_id")] string clientAuthId, 
        CancellationToken cancellationToken);
}
