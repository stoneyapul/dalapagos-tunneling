namespace Dalapagos.Tunneling.Core.Commands;

using Behaviours;
using Model;

[CommandAuthorization(AccessType.Admin)]
public sealed class UpdateDeviceGroupCommand(Guid id, Guid organizationId, Guid userId, string name, ServerStatus serverStatus) 
    : CommandBase<OperationResult<DeviceGroup>>(organizationId, userId)
{ 
    public Guid Id { get; init; } = id;
    public string Name { get; init; } = name;
    public ServerStatus ServerStatus { get; init; } = serverStatus;
}