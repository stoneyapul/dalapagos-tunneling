namespace Dalapagos.Tunneling.Core.Model;

public interface ICommandIdentity
{
    Guid UserId { get; init; }
    Guid OrganizationId { get; init; }
}
