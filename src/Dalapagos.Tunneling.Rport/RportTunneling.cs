namespace Dalapagos.Tunneling.Rport;

using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Client;
using Core;
using Core.Exceptions;
using Core.Extensions;
using Core.Infrastructure;
using Core.Model;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Refit;

public class RportTunneling(IRportPairingClient rportPairingClient, ISecrets secrets, ILogger<RportTunneling> logger) : ITunnelingProvider
{
    private const string Username = "admin";
    private const string ConnectedState = "connected";
    private const string ConnectedErrMsg = "{0} is not connected to the RPort server.";

    public async Task<Tunnel> AddTunnelAsync(
        Guid hubId,
        Guid deviceId, 
        string baseAddress,
        Protocol protocol, 
        ushort port, 
        int? deleteAfterMin, 
        string? allowedIp = null, 
        CancellationToken cancellationToken = default)
    {
        var retryPipeline = GetRetryPipeline();

        try
        {        
            var rportTunnelClient = await CreateRportTunnelClientAsync(hubId, baseAddress, cancellationToken);

            var rportClient = await retryPipeline.ExecuteAsync(async (ct) => await rportTunnelClient.GetClientById(deviceId.ToString(), ct), cancellationToken)
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

    public async Task<TunnelServer> GetServerInformationAsync(
        Guid hubId,
        string baseAddress,
        CancellationToken cancellationToken = default)
    {
        var retryPipeline = GetRetryPipeline();

        try
        {
            var rportTunnelClient = await CreateRportTunnelClientAsync(hubId, baseAddress, cancellationToken);
            var serverStatus = await retryPipeline.ExecuteAsync(async (ct) => await rportTunnelClient.GetServer(ct), cancellationToken);

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

    public async Task<IList<Tunnel>> GetTunnelsByDeviceIdAsync(
        Guid hubId,
        Guid deviceId, 
        string baseAddress,
        CancellationToken cancellationToken = default)
    {
        var tunnels = new List<Tunnel>();
        var retryPipeline = GetRetryPipeline();

        try
        {
            var rportTunnelClient = await CreateRportTunnelClientAsync(hubId, baseAddress, cancellationToken);

            var rportClient = await retryPipeline.ExecuteAsync(async (ct) => await rportTunnelClient.GetClientById(deviceId.ToString(), ct), cancellationToken)
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

    public async Task<bool> IsDeviceConnectedAsync(
        Guid hubId,
        Guid deviceId, 
        string baseAddress,
        CancellationToken cancellationToken = default)
    {
        var retryPipeline = GetRetryPipeline();

        try
        {
            var rportTunnelClient = await CreateRportTunnelClientAsync(hubId, baseAddress, cancellationToken);
            var rportClient = await retryPipeline.ExecuteAsync(async (ct) => await rportTunnelClient.GetClientById(deviceId.ToString(), ct), cancellationToken);

            return !string.IsNullOrWhiteSpace(rportClient.Data.ConnectionState)
                && rportClient.Data.ConnectionState.Equals(ConnectedState);
        }
        catch (ApiException ex)
        { 
            logger.LogError("Message: {message} {content}", ex.RequestMessage, ex.Content);
            throw new TunnelingException(GetErrorMessage(ex), ex.StatusCode);
        }
    }

    public async Task RemoveTunnelAsync(
        Guid hubId,
        Guid deviceId, 
        string baseAddress,
        string tunnelId, 
        CancellationToken cancellationToken = default)
    {
        var retryPipeline = GetRetryPipeline();

        try
        {
            var rportTunnelClient = await CreateRportTunnelClientAsync(hubId, baseAddress, cancellationToken);
            var rportClient = await retryPipeline.ExecuteAsync(async (ct) => await rportTunnelClient.GetClientById(deviceId.ToString(), ct), cancellationToken);

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

    public async Task<string?> ConfigureDeviceConnectionAsync(
        Guid hubId,
        Guid deviceId, 
        string serverBaseAddress,
        string serverFingerprint,
        string clientCredentialString,
        Os os,
        CancellationToken cancellationToken = default)
    {
        var credentials = clientCredentialString.Split(":");
        if (credentials == Array.Empty<string>() || credentials.Length != 2 || !credentials[0].Equals(deviceId.ToString()))
        {
            throw new TunnelingException("Invalid credential string.", System.Net.HttpStatusCode.BadRequest);
        }

        var retryPipeline = GetRetryPipeline();

        try
        {
            var rportTunnelClient = await CreateRportTunnelClientAsync(hubId, serverBaseAddress, cancellationToken);

            var authData = new RportClientAuthData{
                ClientAuthId = deviceId.ToString(),
                Password = credentials[1]
            };

            await retryPipeline.ExecuteAsync(async (ct) => await rportTunnelClient.AddClientAuth(authData, ct), cancellationToken);

            var pairingResponse = await retryPipeline.ExecuteAsync(async (ct) => await rportPairingClient.PairClient(
                new PairingRequest
                {
                    ClientAuthId = deviceId.ToString(),
                    Password = credentials[1],
                    ConnectUrl = $"https://{serverBaseAddress}",
                    Fingerprint = serverFingerprint
                }, ct), cancellationToken);

            var installer = os switch
            {
                Os.Linux => pairingResponse.Installers.Linux,
                Os.Windows => pairingResponse.Installers.Windows,
                _ => throw new TunnelingException("Unsupported OS.", System.Net.HttpStatusCode.BadRequest)
            };

            return installer!;
        }
        catch (ApiException ex)
        {
            logger.LogError("Message: {message} {content}", ex.RequestMessage, ex.Content);
            throw new TunnelingException(GetErrorMessage(ex), ex.StatusCode);
        }
    }

    public async Task RemoveDeviceCredentialsAsync(
        Guid hubId,
        Guid deviceId, 
        string baseAddress,
        CancellationToken cancellationToken = default)
    {
        var retryPipeline = GetRetryPipeline();

        try
        {
            var rportTunnelClient = await CreateRportTunnelClientAsync(hubId, baseAddress, cancellationToken);
            await retryPipeline.ExecuteAsync(async (ct) => await rportTunnelClient.RemoveClientAuth(deviceId.ToString(), ct), cancellationToken);
        }
        catch (ApiException ex)
        {
            logger.LogError("Message: {message} {content}", ex.RequestMessage, ex.Content);
            throw new TunnelingException(GetErrorMessage(ex), ex.StatusCode);
        }
    }

    private static string GetErrorMessage(ApiException ex)
    {
        return ex.Message;
        const string defaultMessage = "Unknown error from a downstream server.";

        if (ex == null)
        {
            return defaultMessage;
        }

        if (string.IsNullOrWhiteSpace(ex.Content))
        {
            return ex.Message;
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

    private static ResiliencePipeline GetRetryPipeline()
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true
            })
            .Build();
    }

    private async Task<IRportTunnelClient> CreateRportTunnelClientAsync(Guid hubId, string baseAddress, CancellationToken cancellationToken = default)
    {
        var serverPassword = await secrets.GetSecretAsync($"{hubId.ToShortHubId()}{Constants.TunnelingServerPassNameSfx}", cancellationToken)
            ?? throw new DataNotFoundException($"Failed to retrieve tunneling server password for hub with id {hubId}");
        var encodedAuthBytes = System.Text.Encoding.UTF8.GetBytes($"{Username}:{serverPassword}");
        var encodedAuth = Convert.ToBase64String(encodedAuthBytes);

        var refitSettings = new RefitSettings { ContentSerializer = new SystemTextJsonContentSerializer() };
        var httpClient = RestService.CreateHttpClient($"https://{baseAddress}", refitSettings);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedAuth);
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        return RestService.For<IRportTunnelClient>(httpClient, refitSettings);
    }
}
