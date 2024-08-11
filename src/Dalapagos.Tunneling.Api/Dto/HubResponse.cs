namespace Dalapagos.Tunneling.Api.Dto;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a response with hub information. 
/// </summary>
public class HubResponse
{
    /// <summary>
    /// A globally unique identifier for the hub.
    /// </summary>
    [JsonPropertyName("hubId")]
    public Guid HubId { get; set; }

    /// <summary>
    /// A device group name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// A count of connected devices.
    /// </summary>
    [JsonPropertyName("connectedDevices")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ConnectedDevices { get; set; }

    /// <summary>
    /// A count of devices, connected and not connected.
    /// </summary>
    [JsonPropertyName("totalDevices")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? TotalDevices { get; set; }

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
    /// A list of devices that belong to the hub.
    /// </summary>
    [JsonPropertyName("devices")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<DeviceResponse>? Devices { get; set; }
}
