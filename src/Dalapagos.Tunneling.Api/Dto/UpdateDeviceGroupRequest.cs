namespace Dalapagos.Tunneling.Api;

using System.ComponentModel.DataAnnotations;

public class UpdateDeviceGroupRequest
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(64, MinimumLength = 1)]    
    public string Name { get; set; } = default!;
}
