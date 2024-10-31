namespace Dalapagos.Tunneling.Core.Commands;

using Behaviours;
using Model;

[CommandAuthorization(AccessType.Admin)]
public sealed class AddDeviceCommand(Guid? id, Guid? hubId, string name, Os os, RestProtocol? restProtocol, ushort? restPort, Guid organizationId, Guid userId) 
    : CommandBase<OperationResult<Device>>(organizationId, userId)
{
    public Guid? Id { get; init; } = id;
    public Guid? HubId { get; init; } = hubId;
    public string Name { get; init; } = name;
    public Os Os { get; init; } = os;
    public RestProtocol? RestProtocol { get; init; } = restProtocol;
    public ushort? RestPort { get; init; } = restPort;
}