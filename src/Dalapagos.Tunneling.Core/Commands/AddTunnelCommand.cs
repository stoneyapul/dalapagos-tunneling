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

}
