namespace Dalapagos.Tunneling.Api.Dto;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a device.
/// </summary>
public class DeviceResponse
{
    /// <summary>
    /// A globally unique identifier for the device.
    /// </summary>
    [JsonPropertyName("deviceId")]
    public Guid DeviceId { get; set; }

    /// <summary>
    /// A globally unique identifier for the device group.
    /// </summary>
    [JsonPropertyName("deviceGroupId")]
    public Guid? DeviceGroupId { get; set; }

    /// <summary>
    /// A device name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// The operating system that the device is running. Linux or Windows.
    /// </summary>
    [JsonPropertyName("os")]
    public string Os { get; set; } = default!;
}
