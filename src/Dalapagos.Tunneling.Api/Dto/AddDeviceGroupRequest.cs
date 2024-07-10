namespace Dalapagos.Tunneling.Api.Dto;

using System.ComponentModel.DataAnnotations;
using Core.Model;
using Validation;

public class AddDeviceGroupRequest
{
    [Required]
    public Guid OrganizationId { get; set; }

    public Guid? DeviceGroupId { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(64, MinimumLength = 1)]    
    public string Name { get; set; } = default!;

    [ValidEnum<ServerLocation>]  
    public string Location { get; set; } = default!;
}
