using System;
using System.Collections.Generic;

namespace Dalapagos.Tunneling.Repository.EF;

public partial class Device
{
    public int DeviceId { get; set; }

    public Guid? DeviceUuid { get; set; }

    public int? DeviceGroupId { get; set; }

    public string DeviceName { get; set; } = null!;

    public int Os { get; set; }

    public virtual DeviceGroup? DeviceGroup { get; set; }
}
