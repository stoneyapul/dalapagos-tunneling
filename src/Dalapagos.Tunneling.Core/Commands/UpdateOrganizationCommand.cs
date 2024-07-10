namespace Dalapagos.Tunneling.Core.Commands;

using Behaviours;
using Model;

[CommandAuthorization(AccessType.Admin)]
public sealed class UpdateOrganizationCommand(Guid id, string name, Guid organizationId, Guid userId)     
    : CommandBase<OperationResult<Organization>>(organizationId, userId)
{
    public Guid Id { get; init; } = id;
    public string Name { get; init; } = name;
}