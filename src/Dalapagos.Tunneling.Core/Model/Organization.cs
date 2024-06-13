namespace Dalapagos.Tunneling.Core.Model;

public record Organization(Guid? Id, string Name, IList<DeviceGroup>? DeviceGroups = null);
