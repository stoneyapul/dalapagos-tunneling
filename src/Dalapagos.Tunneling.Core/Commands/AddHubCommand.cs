namespace Dalapagos.Tunneling.Core.Commands;

using Behaviours;
using Model;

[CommandAuthorization(AccessType.Admin)]
public sealed class AddHubCommand(Guid? id, Guid organizationId, Guid userId, string name, ServerLocation location) 
    : CommandBase<OperationResult<Hub>>(organizationId, userId)
{ 
    public Guid? Id { get; init; } = id;
    public string Name { get; init; } = name;
    public ServerLocation Location { get; init; } = location;
}
