namespace Dalapagos.Tunneling.Core.Extensions;

public static class GuidExtensions
{
   public static string ToShortHubId(this Guid hubId)
    {
        return hubId.ToString().Substring(24, 12).ToLowerInvariant();
    }
}
