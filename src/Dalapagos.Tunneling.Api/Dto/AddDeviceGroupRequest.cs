namespace Dalapagos.Tunneling.Api.Dto;

using System.ComponentModel.DataAnnotations;
using Core.Model;
using Validation;

/// <summary>
/// Represents a request to add a device group. 
/// This takes a few minutes because a tunneling server is provisioned for the device group.
/// </summary>
public class AddDeviceGroupRequest
{
    /// <summary>
    /// A device group id. If not provided, the device group id will be generated.
    /// </summary>
    public Guid? DeviceGroupId { get; set; }

    /// <summary>
    /// A device group name.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [StringLength(64, MinimumLength = 1)]    
    public string Name { get; set; } = default!;

    /// <summary>
    /// A region in the US where the tunneling server is provisioned. West, Central, or East.
    /// </summary>
    [ValidEnum<ServerLocation>]  
    public string Location { get; set; } = default!;
}
