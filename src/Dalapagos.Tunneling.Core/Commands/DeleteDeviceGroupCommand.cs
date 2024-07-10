﻿namespace Dalapagos.Tunneling.Core.Commands;

using Behaviours;
using Model;

[CommandAuthorization(AccessType.Admin)]
public sealed class DeleteDeviceGroupCommand(Guid id, Guid organizationId, Guid userId)     
    : CommandBase<OperationResult>(organizationId, userId)
{
    public Guid Id { get; init; } = id;
}