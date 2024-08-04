namespace Dalapagos.Tunneling.Core.Queries;

using Behaviours;
using Model;

[CommandAuthorization(AccessType.User)]
public sealed class GetHubByIdQuery(Guid id, Guid organizationId, Guid userId) 
    : QueryBase<OperationResult<Hub>>(organizationId, userId)
{
    public Guid Id { get; init; } = id;
}