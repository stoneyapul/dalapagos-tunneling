namespace Dalapagos.Tunneling.Core.Commands;

using Behaviours;
using Model;

[CommandAuthorization(AccessType.Admin)]
public sealed class UpdateHubCommand(Guid id, Guid organizationId, Guid userId, string name) 
    : CommandBase<OperationResult<Hub>>(organizationId, userId)
{ 
    public Guid Id { get; init; } = id;
    public string Name { get; init; } = name;
}