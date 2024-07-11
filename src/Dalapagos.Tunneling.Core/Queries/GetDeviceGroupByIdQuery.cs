namespace Dalapagos.Tunneling.Core.Queries;

using Behaviours;
using Model;

[CommandAuthorization(AccessType.User)]
public sealed class GetDeviceGroupByIdQuery(Guid id, Guid organizationId, Guid userId) 
    : QueryBase<OperationResult<DeviceGroup>>(organizationId, userId)
{
    public Guid Id { get; init; } = id;
}