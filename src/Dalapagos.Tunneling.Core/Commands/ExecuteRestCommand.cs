namespace Dalapagos.Tunneling.Core.Commands;

using Behaviours;
using Model;

[CommandAuthorization(AccessType.Admin)]
public sealed class ExecuteRestCommand(    
    Guid deviceId, 
    string action,
    Guid organizationId, 
    Guid userId)
    : CommandBase<OperationResult<string?>>(organizationId, userId)
{
    public Guid DeviceId { get; init; } = deviceId;

    public string Action { get; init; } = action;
}