namespace Dalapagos.Tunneling.Core.Model;

public record Device(Guid? Id, Guid? HubId, string Name, Os Os)
{
    public string? ConnectionScript { get; set; }
}
