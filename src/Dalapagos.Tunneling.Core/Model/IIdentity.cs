namespace Dalapagos.Tunneling.Core.Model;

public interface IIdentity
{
    Guid OrganizationId { get; set; }
    Guid UserId { get; set; }
}
