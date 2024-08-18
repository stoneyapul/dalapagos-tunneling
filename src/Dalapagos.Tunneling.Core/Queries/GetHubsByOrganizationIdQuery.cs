namespace Dalapagos.Tunneling.Core.Queries;

using Behaviours;
using Model;

[CommandAuthorization(AccessType.User)]
public sealed class GetHubsByOrganizationIdQuery(Guid organizationId, Guid userId) 
    : QueryBase<OperationResult<IList<Hub>>>(organizationId, userId)
{
}   