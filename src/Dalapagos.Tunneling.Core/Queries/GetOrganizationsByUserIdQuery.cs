namespace Dalapagos.Tunneling.Core.Queries;

using Behaviours;
using Model;

[CommandAuthorization(AccessType.User)]
public sealed class GetOrganizationsByUserIdQuery(Guid userId) 
: QueryBase<OperationResult<IList<Organization>>>(default, userId)
{
}
