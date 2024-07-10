namespace Dalapagos.Tunneling.Core.Commands;

using Behaviours;
using Model;

[CommandAuthorization(AccessType.Admin)]
public sealed class AddDeviceCommand(Guid? id, Guid? deviceGroupId, string name, Os os, Guid organizationId, Guid userId) 
    : CommandBase<OperationResult<Device>>(organizationId, userId)
{
    public Guid? Id { get; init; } = id;
    public Guid? DeviceGroupId { get; init; } = deviceGroupId;
    public string Name { get; init; } = name;
    public Os Os { get; init; } = os;
}