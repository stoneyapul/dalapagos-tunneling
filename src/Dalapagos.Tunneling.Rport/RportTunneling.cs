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
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Refit;

public class RportTunneling(ITunnelingRepository tunnelingRepository, ILogger<RportTunneling> logger) : ITunnelingProvider
{
    private const int MaxRetries = 2;
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
            var (rportTunnelClient, baseAddress) = await CreateRportTunnelClient(deviceId, cancellationToken);

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
        catch (ApiException ex)
        {
            logger.LogError("Message: {message} {content}", ex.RequestMessage, ex.Content);
            throw new TunnelingException(GetErrorMessage(ex), ex.StatusCode);
        }
    }

    public async Task<TunnelServer> GetServerInformationAsync(Guid organizationId, Guid deviceGroupId, CancellationToken cancellationToken = default)
    {
        try
        {
            var (rportTunnelClient, baseAddress) = await CreateRportTunnelClient(organizationId, deviceGroupId, cancellationToken);

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
            var (rportTunnelClient, baseAddress) = await CreateRportTunnelClient(deviceId, cancellationToken);

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
        catch (ApiException ex)
        {
            logger.LogError("Message: {message} {content}", ex.RequestMessage, ex.Content);
            throw new TunnelingException(GetErrorMessage(ex), ex.StatusCode);
        }
    }

    public async Task<bool> IsDeviceConnectedAsync(Guid deviceId, CancellationToken cancellationToken = default)
    {
        try
        {
            var (rportTunnelClient, baseAddress) = await CreateRportTunnelClient(deviceId, cancellationToken);

            var rportClient = await rportTunnelClient.GetClientById(deviceId.ToString(), cancellationToken);
            return !string.IsNullOrWhiteSpace(rportClient.Data.ConnectionState)
                && rportClient.Data.ConnectionState.Equals(ConnectedState);
        }
        catch (ApiException ex)
        {
            logger.LogError("Message: {message} {content}", ex.RequestMessage, ex.Content);
            throw new TunnelingException(GetErrorMessage(ex), ex.StatusCode);
        }
    }

    public async Task RemoveTunnelAsync(Guid deviceId, string tunnelId, CancellationToken cancellationToken = default)
    {
        try
        {
            var (rportTunnelClient, baseAddress) = await CreateRportTunnelClient(deviceId, cancellationToken);

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
        catch (ApiException ex)
        {
            logger.LogError("Message: {message} {content}", ex.RequestMessage, ex.Content);
            throw new TunnelingException(GetErrorMessage(ex), ex.StatusCode);
        }
    }

    public async Task RemoveTunnelCredentialsAsync(Guid deviceId, CancellationToken cancellationToken = default)
    {
        try
        {
            var (rportTunnelClient, baseAddress) = await CreateRportTunnelClient(deviceId, cancellationToken);
            await rportTunnelClient.RemoveClientAuth(deviceId.ToString(), cancellationToken);
        }
        catch (ApiException ex)
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

    // TODO: Figure out how to best use this retry policy.
    private static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                MaxRetries,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (result, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine(
                        $"A non success code {(int?)result.Result?.StatusCode} was received on retry {retryCount}."
                    );
                }
            );
    }


    private async Task<(IRportTunnelClient Client, string BaseAddress)> CreateRportTunnelClient(Guid deviceId, CancellationToken cancellationToken = default)
    {
        var refitSettings = new RefitSettings { ContentSerializer = new SystemTextJsonContentSerializer() };
        var baseAddress = await tunnelingRepository.RetrieveServerBaseAddressByDeviceIdAsync(deviceId, cancellationToken);

        return (RestService.For<IRportTunnelClient>($"https://{baseAddress}", refitSettings), baseAddress);
    }

    private async Task<(IRportTunnelClient Client, string BaseAddress)> CreateRportTunnelClient(Guid organizationId, Guid deviceGroupId, CancellationToken cancellationToken = default)
    {
        var refitSettings = new RefitSettings { ContentSerializer = new SystemTextJsonContentSerializer() };
        var deviceGroup = await tunnelingRepository.RetrieveDeviceGroupAsync(organizationId, deviceGroupId, cancellationToken) 
            ?? throw new DataNotFoundException($"Device group {deviceGroupId} not found.");

        if (string.IsNullOrWhiteSpace(deviceGroup.ServerBaseUrl))
        {
            throw new DataNotFoundException($"Failed to retrieve tunneling server base address for device group {deviceGroup.Name}.");
        }

        return (RestService.For<IRportTunnelClient>($"https://{deviceGroup.ServerBaseUrl}", refitSettings), deviceGroup.ServerBaseUrl);
    }
}
