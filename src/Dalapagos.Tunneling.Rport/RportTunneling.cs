﻿namespace Dalapagos.Tunneling.Rport;

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

public class RportTunneling(ITunnelingRepository tunnelingRepository, ISecrets secrets, ILogger<RportTunneling> logger) : ITunnelingProvider
{
    private const string Username = "admin";
    private const string ConnectedState = "connected";
    private const string ConnectedErrMsg = "{0} is not connected to the RPort server.";

    public async Task<Tunnel> AddTunnelAsync(
        Guid organizationId,
        Guid deviceId, 
        Protocol protocol, 
        ushort port, 
        int? deleteAfterMin, 
        string? allowedIp = null, 
        CancellationToken cancellationToken = default)
    {
        var retryPipeline = GetRetryPipeline();

        try
        {        
            var (rportTunnelClient, baseAddress) = await CreateRportTunnelClientByDeviceIdAsync(organizationId, deviceId, cancellationToken);

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

    public async Task<TunnelServer> GetServerInformationAsync(Guid organizationId, Guid deviceGroupId, CancellationToken cancellationToken = default)
    {
        try
        {
            var (rportTunnelClient, baseAddress) = await CreateRportTunnelClientByDeviceGroupIdAsync(organizationId, deviceGroupId, cancellationToken);

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

    public async Task<IList<Tunnel>> GetTunnelsByDeviceIdAsync(Guid organizationId, Guid deviceId, CancellationToken cancellationToken = default)
    {
        var tunnels = new List<Tunnel>();

        try
        {
            var (rportTunnelClient, baseAddress) = await CreateRportTunnelClientByDeviceIdAsync(organizationId, deviceId, cancellationToken);

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

    public async Task<bool> IsDeviceConnectedAsync(Guid organizationId, Guid deviceId, CancellationToken cancellationToken = default)
    {
        try
        {
            var (rportTunnelClient, baseAddress) = await CreateRportTunnelClientByDeviceIdAsync(organizationId, deviceId, cancellationToken);

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

    public async Task RemoveTunnelAsync(Guid organizationId, Guid deviceId, string tunnelId, CancellationToken cancellationToken = default)
    {
        try
        {
            var (rportTunnelClient, baseAddress) = await CreateRportTunnelClientByDeviceIdAsync(organizationId, deviceId, cancellationToken);

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

    public async Task RemoveTunnelCredentialsAsync(Guid organizationId, Guid deviceId, CancellationToken cancellationToken = default)
    {
        try
        {
            var (rportTunnelClient, baseAddress) = await CreateRportTunnelClientByDeviceIdAsync(organizationId, deviceId, cancellationToken);
            await rportTunnelClient.RemoveClientAuth(deviceId.ToString(), cancellationToken);
        }
        catch (ApiException ex)
        {
            logger.LogError("Message: {message} {content}", ex.RequestMessage, ex.Content);
            throw new TunnelingException(GetErrorMessage(ex), ex.StatusCode);
        }
    }

    private static string GetErrorMessage(ApiException ex)
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

    private async Task<(IRportTunnelClient Client, string BaseAddress)> CreateRportTunnelClientByDeviceIdAsync(
        Guid organizationId, 
        Guid deviceId, 
        CancellationToken cancellationToken = default)
    {
        var deviceGroup = await tunnelingRepository.RetrieveDeviceGroupByDeviceIdAsync(organizationId, deviceId, cancellationToken)
            ?? throw new DataNotFoundException($"Device group for device id {deviceId} not found.");

        var client = await CreateRportTunnelClientAsync(deviceGroup, cancellationToken);

        return (client, deviceGroup.ServerBaseUrl!);
    }

    private async Task<(IRportTunnelClient Client, string BaseAddress)> CreateRportTunnelClientByDeviceGroupIdAsync(
        Guid organizationId, 
        Guid deviceGroupId, 
        CancellationToken cancellationToken = default)
    {
        var deviceGroup = await tunnelingRepository.RetrieveDeviceGroupAsync(organizationId, deviceGroupId, cancellationToken) 
            ?? throw new DataNotFoundException($"Device group {deviceGroupId} not found.");

        var client = await CreateRportTunnelClientAsync(deviceGroup, cancellationToken);

        return (client, deviceGroup.ServerBaseUrl!);
    }

    private async Task<IRportTunnelClient> CreateRportTunnelClientAsync(DeviceGroup deviceGroup, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(deviceGroup, nameof(deviceGroup));
        ArgumentNullException.ThrowIfNull(deviceGroup.Id, nameof(deviceGroup.Id));
        ArgumentException.ThrowIfNullOrWhiteSpace(deviceGroup.ServerBaseUrl, nameof(deviceGroup.ServerBaseUrl));

        var serverPassword = await secrets.GetSecretAsync($"{deviceGroup.Id.Value.ToShortDeviceGroupId()}{Constants.TunnelingServerPassNameSfx}", cancellationToken)
            ?? throw new DataNotFoundException($"Failed to retrieve tunneling server password for device group {deviceGroup.Name}.");

        var encodedAuthBytes = System.Text.Encoding.UTF8.GetBytes($"{Username}:{serverPassword}");
        var encodedAuth = Convert.ToBase64String(encodedAuthBytes);

        var refitSettings = new RefitSettings { ContentSerializer = new SystemTextJsonContentSerializer() };
        var httpClient = RestService.CreateHttpClient($"https://{deviceGroup.ServerBaseUrl}", refitSettings);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedAuth);
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        httpClient.DefaultRequestVersion = new Version(1, 3);

        return RestService.For<IRportTunnelClient>(httpClient, refitSettings);
    }
}
