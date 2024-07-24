namespace Dalapagos.Tunneling.Api.Dto;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Core.Model;
using Validation;

/// <summary>
/// Represents a request to create a tunnel.
/// </summary>
public class AddTunnelRequest
{
    /// <summary>
    /// A globally unique identifier for the device.
    /// </summary>
    [JsonPropertyName("deviceId")]
    [Required(AllowEmptyStrings = false)]
    public Guid DeviceId { get; set; }

    ///// <summary>
    ///// The protocol. Ssh or Https.
    ///// </summary>
    [JsonPropertyName("protocol")]
    [ValidEnum<Protocol>]  
     public string Protocol { get; set; } = default!;

    /// <summary>
    /// The port to use on the controller. If not provided, a default port is assigned.
    /// </summary>
    [JsonPropertyName("port")]
    public ushort? Port { get; set; }

    ///// <summary>
    ///// The IP address that will be allowed to use the tunnel. This should be the caller's IP address.
    ///// </summary>
    [JsonPropertyName("allowedIp")]
    public string? AllowedIp { get; set; }

    /// <summary>
    /// Delete the tunnel after this number of minutes.
    /// </summary>
    [JsonPropertyName("deleteAfterMin")]
    public int DeleteAfterMin { get; set; } = 60;
}
