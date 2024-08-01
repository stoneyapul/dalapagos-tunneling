using System;
using System.Collections.Generic;

namespace Dalapagos.Tunneling.Repository.EF;

public partial class Organization
{
    public int OrganizationId { get; set; }

    public string OrganizationName { get; set; } = null!;

    public Guid OrganizationUuid { get; set; }

    public virtual ICollection<DeviceGroup> DeviceGroups { get; set; } = new List<DeviceGroup>();

    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();

    public virtual ICollection<OrganizationUser> OrganizationUsers { get; set; } = new List<OrganizationUser>();
}
