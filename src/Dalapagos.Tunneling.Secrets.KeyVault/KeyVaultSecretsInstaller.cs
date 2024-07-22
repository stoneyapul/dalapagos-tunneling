namespace Dalapagos.Tunneling.Secrets.KeyVault;

using Microsoft.Extensions.DependencyInjection;

public static class KeyVaultSecretsInstaller
{
    public static void AddKeyVaultSecrets(this IServiceCollection services)
    {
        services.AddScoped<Core.Infrastructure.ISecrets, KeyVaultSecrets>();
    }
}
