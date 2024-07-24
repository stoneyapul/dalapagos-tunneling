namespace Dalapagos.Tunneling.Api.Dto;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a response with device group information. 
/// </summary>
public class DeviceGroupResponse
{
    /// <summary>
    /// A globally unique identifier for the device group.
    /// </summary>
    [JsonPropertyName("deviceGroupId")]
    public Guid DeviceGroupId { get; set; }

    /// <summary>
    /// A device group name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// A region in the US where the tunneling server is provisioned. West, Central, or East.
    /// </summary>
    [JsonPropertyName("location")]
    public string Location { get; set; } = default!;

    /// <summary>
    /// The status of the tunneling server. Unknown, Deploying, Deployed, DeployFailed, Online, or Error.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = default!;

    /// <summary>
    /// A list of devices that belong to the device group.
    /// </summary>
    [JsonPropertyName("devices")]
    public IList<DeviceResponse>? Devices { get; set; }
}
