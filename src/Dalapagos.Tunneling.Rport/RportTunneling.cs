namespace Dalapagos.Tunneling.Rport;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Client;
using Core.Exceptions;
using Core.Infrastructure;
using Core.Model;
using Microsoft.Extensions.Logging;

public class RportTunneling( 
    ITunnelingRepository tunnelingRepository,  
    IRportTunnelClient rportTunnelClient,
    ILogger<RportTunneling> logger) : ITunneling
{
    private const string ConnectedState = "connected";
    private const string ConnectedErrMsg = "{0} is not connected to the RPort server.";

    public async Task<Tunnel> AddTunnelAsync(
        Guid deviceId, 
        Protocol protocol, 
        ushort port, 
        int? deleteAfterMin, 
        string? allowedIp = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var baseAddress = await tunnelingRepository.RetrieveServerBaseAddressAsync(deviceId, cancellationToken)
                ?? throw new TunnelingException("Failed to retrieve server base address.", System.Net.HttpStatusCode.InternalServerError);

            var rportClient = await rportTunnelClient.GetClientById(deviceId.ToString(), cancellationToken)
                ?? throw new TunnelingException("Failed to retrieve tunneling client information.", System.Net.HttpStatusCode.InternalServerError);

            if (
                string.IsNullOrWhiteSpace(rportClient.Data.ConnectionState)
                || !rportClient.Data.ConnectionState.Equals(ConnectedState)
            )
            {
                throw new TunnelingException(
                    string.Format(ConnectedErrMsg, deviceId),
                    System.Net.HttpStatusCode.NotFound
                );
            }

            var queryString = new AddTunnelQueryParams( 
                port.ToString(),
                protocol.ToString().ToLowerInvariant(),
                deleteAfterMin,
                allowedIp != null ? [allowedIp] : null
            );

            var rportTunnel = await rportTunnelClient.AddClientTunnel(
                deviceId.ToString(),
                queryString,
                cancellationToken
            ) ?? throw new TunnelingException("Failed to create tunnel.", System.Net.HttpStatusCode.InternalServerError);

            return TunnelMapper.Map(rportTunnel.Data, baseAddress)
                ?? throw new TunnelingException("Failed to create tunnel.", System.Net.HttpStatusCode.InternalServerError);
        }
        catch (Refit.ApiException ex)
        {
            logger.LogError("Message: {message} {content}", ex.RequestMessage, ex.Content);
            throw new TunnelingException(GetErrorMessage(ex), ex.StatusCode);
        }
    }

    public async Task<TunnelServer> GetServerInformationAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var serverStatus = await rportTunnelClient.GetServer(cancellationToken);
            return new TunnelServer(
                ServerStatus.Online, 
                null,
                serverStatus.Data.Version,
                serverStatus.Data.ClientsConnected
            );
        }
        catch (Exception ex)
        {
            logger.LogError("Message: {message}", ex.Message);
            return new TunnelServer(ServerStatus.Error, ex.Message, null, null);
        }
    }

    public async Task<IList<Tunnel>> GetTunnelsByDeviceIdAsync(Guid deviceId, CancellationToken cancellationToken = default)
    {
        var tunnels = new List<Tunnel>();

        try
        {
            var baseAddress = await tunnelingRepository.RetrieveServerBaseAddressAsync(deviceId, cancellationToken)
                ?? throw new TunnelingException("Failed to retrieve server base address.", System.Net.HttpStatusCode.InternalServerError);

            var rportClient = await rportTunnelClient.GetClientById(deviceId.ToString(), cancellationToken)
                ?? throw new TunnelingException("Failed to retrieve tunneling client information.", System.Net.HttpStatusCode.InternalServerError);

           if (
                string.IsNullOrWhiteSpace(rportClient.Data.ConnectionState)
                || !rportClient.Data.ConnectionState.Equals(ConnectedState)
            )
            {
                throw new TunnelingException(
                    string.Format(ConnectedErrMsg, deviceId),
                    System.Net.HttpStatusCode.NotFound
                );
            }

            foreach (var rportTunnel in rportClient.Data.Tunnels)
            {
                var tunnel = TunnelMapper.Map(rportTunnel, baseAddress);
                if (tunnel == null)
                {
                    continue;
                }

                tunnels.Add(tunnel);
            }

            return tunnels;
        }
        catch (Refit.ApiException ex)
        {
            logger.LogError("Message: {message} {content}", ex.RequestMessage, ex.Content);
            throw new TunnelingException(GetErrorMessage(ex), ex.StatusCode);
        }
    }

    public async Task<bool> IsDeviceConnectedAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        try
        {
            var rportClient = await rportTunnelClient.GetClientById(deviceId.ToString(), cancellationToken);
            return !string.IsNullOrWhiteSpace(rportClient.Data.ConnectionState)
                && rportClient.Data.ConnectionState.Equals(ConnectedState);
        }
        catch (Refit.ApiException ex)
        {
            logger.LogError("Message: {message} {content}", ex.RequestMessage, ex.Content);
            throw new TunnelingException(GetErrorMessage(ex), ex.StatusCode);
        }
    }

    public async Task RemoveTunnelAsync(Guid deviceId, string tunnelId, CancellationToken cancellationToken = default)
    {
        try
        {
            var rportClient = await rportTunnelClient.GetClientById(deviceId.ToString(), cancellationToken);
            if (
                string.IsNullOrWhiteSpace(rportClient.Data.ConnectionState)
                || !rportClient.Data.ConnectionState.Equals(ConnectedState)
            )
            {
                throw new TunnelingException(
                    string.Format(ConnectedErrMsg, deviceId.ToString()),
                    System.Net.HttpStatusCode.NotFound
                );
            }

            await rportTunnelClient.RemoveClientTunnel(deviceId.ToString(), tunnelId, cancellationToken);
        }
        catch (Refit.ApiException ex)
        {
            logger.LogError("Message: {message} {content}", ex.RequestMessage, ex.Content);
            throw new TunnelingException(GetErrorMessage(ex), ex.StatusCode);
        }
    }

    public async Task RemoveTunnelCredentialsAsync(Guid deviceId, CancellationToken cancellationToken = default)
    {
        try
        {
            await rportTunnelClient.RemoveClientAuth(deviceId.ToString(), cancellationToken);
        }
        catch (Refit.ApiException ex)
        {
            logger.LogError("Message: {message} {content}", ex.RequestMessage, ex.Content);
            throw new TunnelingException(GetErrorMessage(ex), ex.StatusCode);
        }
    }

    private static string GetErrorMessage(Refit.ApiException ex)
    {
        const string defaultMessage = "Unknown error from a downstream server.";

        if (ex == null || string.IsNullOrWhiteSpace(ex.Content))
        {
            return defaultMessage;
        }

        try
        {
            var content = JsonSerializer.Deserialize<RportErrors>(ex.Content);
            return content!.Errors.First().Title ?? defaultMessage;
        }
        catch
        {
            return defaultMessage;
        }
    }
}
