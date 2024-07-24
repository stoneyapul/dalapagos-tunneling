namespace Dalapagos.Tunneling.Core.Commands;

using Behaviours;
using Model;

[CommandAuthorization(AccessType.User)]
public sealed class AddTunnelCommand(
    Guid deviceId, 
    Protocol protocol, 
    int? deleteAfterMin, 
    ushort? devicePort, 
    string? allowedIp,
    Guid organizationId, 
    Guid userId)
    : CommandBase<OperationResult<Tunnel>>(organizationId, userId)
{
    public Guid DeviceId { get; init; } = deviceId;
    public Protocol Protocol { get; init; } = protocol;
    public int? DeleteAfterMin { get; init; } = deleteAfterMin;
    public ushort? DevicePort { get; init; } = devicePort;
    public string? AllowedIp { get; init; } = allowedIp;
}
