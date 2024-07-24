namespace Dalapagos.Tunneling.Api.Dto;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class UpdateDeviceGroupRequest
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(64, MinimumLength = 1)]    
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;
}
