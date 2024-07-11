namespace Dalapagos.Tunneling.Core.Queries;

using System;
using Mediator;
using Model;

public abstract  class QueryBase<TResponse>(Guid organizationId, Guid userId) : ICommandIdentity, IRequest<TResponse>  
    where TResponse : OperationResult
{
    public Guid UserId { get; init; } = userId;
    public Guid OrganizationId { get; init; } = organizationId;
}
