namespace Dalapagos.Tunneling.Core.Model;

public record Organization(Guid? OrganizationId, string OrganizationName, IList<DeviceGroup>? DeviceGroups = null);
