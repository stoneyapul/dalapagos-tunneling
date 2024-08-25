namespace Dalapagos.Tunneling.Api.Dto;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a device pairing script.
/// </summary>
public class DevicePairingScriptResponse
{
    /// <summary>
    /// A script to run on the device for connecting to the tunneling server.
    /// </summary>
    [JsonPropertyName("pairingScript")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PairingScript { get; set; }
}
