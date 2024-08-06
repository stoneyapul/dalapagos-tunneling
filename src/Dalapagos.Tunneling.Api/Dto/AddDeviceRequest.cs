namespace Dalapagos.Tunneling.Api.Dto;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Core.Model;
using Validation;

/// <summary>
/// Represents a request to add a device.
/// </summary>
public class AddDeviceRequest
{
    /// <summary>
    /// A globally unique identifier for the device. If not provided, the device id will be generated.
    /// </summary>
    [JsonPropertyName("deviceId")]
    public Guid? DeviceId { get; set; }

    /// <summary>
    /// A globally unique identifier for the device group. If not provided, the device will not be associated with a device group.
    /// </summary>
    [JsonPropertyName("hubId")]
    public Guid? HubId { get; set; }

    /// <summary>
    /// A device name.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [StringLength(64, MinimumLength = 1)]    
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// The operating system that the device is running. Linux or Windows.
    /// </summary>
    [ValidEnum<Os>]  
    [JsonPropertyName("os")]
    public string Os { get; set; } = default!;
}