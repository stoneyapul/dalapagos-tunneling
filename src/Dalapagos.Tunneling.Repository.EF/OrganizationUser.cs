using System;
using System.Collections.Generic;

namespace Dalapagos.Tunneling.Repository.EF;

public partial class OrganizationUser
{
    public int OrganizationUserId { get; set; }

    public int OrganizationId { get; set; }

    public Guid UserUuid { get; set; }

    public Guid SecurityGroupUuid { get; set; }

    public virtual Organization Organization { get; set; } = null!;
}
