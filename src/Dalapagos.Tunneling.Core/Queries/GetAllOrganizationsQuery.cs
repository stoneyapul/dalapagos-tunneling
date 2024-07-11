namespace Dalapagos.Tunneling.Core.Queries;

using Behaviours;
using Model;

[CommandAuthorization(AccessType.User)]
public sealed class GetAllOrganizationsQuery(Guid organizationId, Guid userId)
    : QueryBase<OperationResult<IList<Organization>>>(organizationId, userId){}
