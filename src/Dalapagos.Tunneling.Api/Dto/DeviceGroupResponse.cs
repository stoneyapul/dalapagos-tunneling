namespace Dalapagos.Tunneling.Api.Dto;

/// <summary>
/// Represents a response with device group information. 
/// </summary>
public class DeviceGroupResponse
{
    /// <summary>
    /// A device group id.
    /// </summary>
    public Guid DeviceGroupId { get; set; }

    /// <summary>
    /// A device group name.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// A region in the US where the tunneling server is provisioned. West, Central, or East.
    /// </summary>
    public string Location { get; set; } = default!;

    /// <summary>
    /// The status of the tunneling server. Unknown, Deploying, Deployed, DeployFailed, Online, or Error.
    /// </summary>
    public string Status { get; set; } = default!;

    /// <summary>
    /// A list of devices that belong to the device group.
    /// </summary>
    public IList<DeviceResponse>? Devices { get; set; }
}
