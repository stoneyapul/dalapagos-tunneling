namespace Dalapagos.Tunneling.Core.Queries;

using Behaviours;
using Model;

[CommandAuthorization(AccessType.Admin)]
public sealed class GetDevicePairingScriptQuery(Guid id, Guid organizationId, Guid userId)
    : QueryBase<OperationResult<string?>>(organizationId, userId)
{
    public Guid Id { get; init; } = id;
}
