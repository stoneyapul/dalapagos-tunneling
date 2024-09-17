namespace Dalapagos.Tunneling.Core.Model;

public record OrganizationUser(Guid OrganizationId, Guid UserId, Guid SecurityGroupId, string OrganizationName);
