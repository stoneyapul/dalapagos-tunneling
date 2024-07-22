namespace Dalapagos.Tunneling.Rport.Client;

using Refit;

public interface IRportTunnelClient
{
    [Get("/status")]
    Task<RportServer> GetServer(CancellationToken cancellationToken);

    [Get("/clients/{client_id}")]
    Task<RportClient> GetClientById(
        [AliasAs("client_id")] string clientId, 
        CancellationToken cancellationToken);

    [Put("/clients/{client_id}/tunnels")]
    Task<RportTunnel> AddClientTunnel(
        [AliasAs("client_id")] string clientId,
        [Query] AddTunnelQueryParams queryString,
        CancellationToken cancellationToken
    );

    [Delete("/clients/{client_id}/tunnels/{tunnel_id}?force=true")]
    Task RemoveClientTunnel(
        [AliasAs("client_id")] string clientId,
        [AliasAs("tunnel_id")] string tunnelId,
        CancellationToken cancellationToken
    );

    [Get("/clients-auth/{client_auth_id}")]
    Task<RportClientAuth> GetClientAuthById(
        [AliasAs("client_auth_id")] string clientAuthId,
        CancellationToken cancellationToken
    );

    [Post("/clients-auth")]
    Task AddClientAuth(
        [AliasAs("client_auth_id")] string clientAuthId,
        RportClientAuthData request,
        CancellationToken cancellationToken
    );

    [Delete("/clients-auth/{client_auth_id}?force=true")]
    Task RemoveClientAuth(
        [AliasAs("client_auth_id")] string clientAuthId, 
        CancellationToken cancellationToken);
}
