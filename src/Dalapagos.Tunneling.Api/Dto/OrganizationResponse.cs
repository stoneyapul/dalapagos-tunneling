namespace Dalapagos.Tunneling.Api.Dto;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a response with organization information. 
/// </summary>
public class OrganizationResponse
{
   /// <summary>
    /// A globally unique identifier for the organization.
    /// </summary>
    [JsonPropertyName("organizationId")]
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// An organization name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;
}