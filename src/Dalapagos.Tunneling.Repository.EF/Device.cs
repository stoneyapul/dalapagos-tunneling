﻿using System;
using System.Collections.Generic;

namespace Dalapagos.Tunneling.Repository.EF;

public partial class Device
{
    public int DeviceId { get; set; }

    public Guid? DeviceUuid { get; set; }

    public int? DeviceGroupId { get; set; }

    public string DeviceName { get; set; } = null!;

    public int Os { get; set; }

    public int OrganizationId { get; set; }

    public string? RestProtocol { get; set; }

    public int? RestPort { get; set; }

    public virtual DeviceGroup? DeviceGroup { get; set; }

    public virtual Organization Organization { get; set; } = null!;
}
