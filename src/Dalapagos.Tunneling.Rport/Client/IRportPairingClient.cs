namespace Dalapagos.Tunneling.Rport.Client;

using Refit;

public interface IRportPairingClient
{
    [Post("")]
    Task<PairingResponse> PairClient(PairingRequest request, CancellationToken cancellationToken = default);
}
