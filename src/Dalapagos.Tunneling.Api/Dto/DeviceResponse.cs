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
    /// A globally unique identifier for the hub.
    /// </summary>
    [JsonPropertyName("hubId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? HubId { get; set; }

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

    /// <summary>
    /// A script to run on the device for connecting to the tunneling server.
    /// </summary>
    [JsonPropertyName("pairingScript")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PairingScript { get; set; }
}
