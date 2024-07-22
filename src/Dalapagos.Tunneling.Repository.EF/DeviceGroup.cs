using System;
using System.Collections.Generic;

namespace Dalapagos.Tunneling.Repository.EF;

public partial class DeviceGroup
{
    public int DeviceGroupId { get; set; }

    public Guid DeviceGroupUuid { get; set; }

    public int OrganizationId { get; set; }

    public string DeviceGroupName { get; set; } = null!;

    public string ServerLocation { get; set; } = null!;

    public int ServerStatus { get; set; }

    public string? ServerBaseUrl { get; set; }

    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();

    public virtual Organization Organization { get; set; } = null!;
}
