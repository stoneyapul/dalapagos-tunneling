namespace Dalapagos.Tunneling.Api.Dto;

using System.ComponentModel.DataAnnotations;
using Core.Model;
using Validation;

public class AddDeviceRequest
{
    public Guid? DeviceId { get; set; }

    public Guid? DeviceGroupId { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(64, MinimumLength = 1)]    
    public string Name { get; set; } = default!;

    [ValidEnum<Os>]  
    public string Os { get; set; } = default!;
}