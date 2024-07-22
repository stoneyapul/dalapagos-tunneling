namespace Dalapagos.Tunneling.Secrets.KeyVault;

using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Core.Exceptions;
using Core.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class KeyVaultSecrets : ISecrets
{
    private readonly ILogger<KeyVaultSecrets> _logger;
    private readonly SecretClient _keyVaultClient;

    public KeyVaultSecrets(ILogger<KeyVaultSecrets> logger, IConfiguration config)
    {
        _logger = logger;

        var keyVaultName = config["KeyVaultName"] ?? throw new ConfigurationException("KeyVaultName");
        var credential = GetTokenCredential(config);
        var keyVaultUrl = "https://" + keyVaultName + ".vault.azure.net";
        _keyVaultClient = new SecretClient(new Uri(keyVaultUrl), credential);
    }

    public async Task<string> GetSecretAsync(string secretName, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting secret {SecretName}.", secretName);
        var response = await _keyVaultClient.GetSecretAsync(secretName, cancellationToken: cancellationToken);

        return response.Value.Value;
    }

    public async Task SetSecretAsync(string secretName, string secretValue, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Saving secret {SecretName}.", secretName);
        await _keyVaultClient.SetSecretAsync(secretName, secretValue, cancellationToken);
    }

    public async Task RemoveSecretAsync(string secretName, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing secret {SecretName}.", secretName);
        await _keyVaultClient.StartDeleteSecretAsync(secretName, cancellationToken: cancellationToken);
    }

    protected static TokenCredential GetTokenCredential(IConfiguration config)
    {
        var adConfigSection = config.GetSection("AzureAd");
        var tenantId = adConfigSection.GetValue<string>("TenantId");
        var clientId = adConfigSection.GetValue<string>("ClientId");
        var clientSecret = adConfigSection.GetValue<string>("ClientSecret");

        return
            string.IsNullOrWhiteSpace(tenantId)
            || string.IsNullOrWhiteSpace(clientId)
            || string.IsNullOrWhiteSpace(clientSecret)
            ? new DefaultAzureCredential()
            : new ClientSecretCredential(tenantId, clientId, clientSecret);
    }
}
