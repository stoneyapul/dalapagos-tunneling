namespace Dalapagos.Tunneling.Api.Dto;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Core.Model;
using Validation;

/// <summary>
/// Represents a request to add a hub. 
/// This takes a few minutes because a tunneling server is provisioned for the hub.
/// </summary>
public class AddHubRequest
{
    /// <summary>
    /// A globally unique identifier for the hub. If not provided, the hub id will be generated.
    /// </summary>
    [JsonPropertyName("hubId")]
    public Guid? HubId { get; set; }

    /// <summary>
    /// A device group name.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [StringLength(64, MinimumLength = 1)]    
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// A region in the US where the tunneling server is provisioned. West, Central, or East.
    /// </summary>
    [ValidEnum<ServerLocation>]  
    [JsonPropertyName("location")]
    public string Location { get; set; } = default!;
}
