namespace Dalapagos.Tunneling.Core.Queries;

using Behaviours;
using Model;

[CommandAuthorization(AccessType.User)]
public sealed class GetDeviceConnectionStatusQuery(Guid id, Guid organizationId, Guid userId) 
    : QueryBase<OperationResult<bool>>(organizationId, userId)
{
    public Guid Id { get; init; } = id;
}