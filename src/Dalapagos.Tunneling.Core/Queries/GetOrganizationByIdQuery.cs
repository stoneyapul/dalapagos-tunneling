namespace Dalapagos.Tunneling.Core.Queries;

using Behaviours;
using Model;

[CommandAuthorization(AccessType.User)]
public sealed class GetOrganizationByIdQuery(Guid id, Guid organizationId, Guid userId) 
    : QueryBase<OperationResult<Organization>>(organizationId, userId)
{
    public Guid Id { get; init; } = id;
}   