namespace Dalapagos.Tunneling.Core.Commands;

using Behaviours;
using Model;

[CommandAuthorization(AccessType.Admin)]
public sealed class ExecuteRestCommand(    
    Guid deviceId, 
    string action,
    string path,
    Guid organizationId, 
    Guid userId)
    : CommandBase<OperationResult<HttpResponseMessage?>>(organizationId, userId)
{
    public Guid DeviceId { get; init; } = deviceId;

    public string Action { get; init; } = action;

    public string Path { get; init; } = path;
}