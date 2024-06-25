namespace Dalapagos.Tunneling.Api.Dto;

using System.ComponentModel.DataAnnotations;
using Validation;

public class AddDeviceRequest
{
    public Guid? DeviceId { get; set; }

    public Guid? DeviceGroupId { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(64)]    
    public string Name { get; set; } = default!;

    [ValidOs]  
    public string Os { get; set; } = default!;
}