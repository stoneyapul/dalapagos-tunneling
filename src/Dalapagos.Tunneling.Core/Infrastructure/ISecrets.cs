namespace Dalapagos.Tunneling.Core.Infrastructure;

public interface ISecrets
{
    Task<string> GetSecretAsync(string secretName, CancellationToken cancellationToken);
    Task SetSecretAsync(string secretName, string secretValue, CancellationToken cancellationToken);

    Task RemoveSecretAsync(string secretName, CancellationToken cancellationToken);
}
