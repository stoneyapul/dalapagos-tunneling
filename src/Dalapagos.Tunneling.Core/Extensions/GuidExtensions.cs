namespace Dalapagos.Tunneling.Core.Extensions;

public static class GuidExtensions
{
   public static string ToShortDeviceGroupId(this Guid deviceGroupId)
    {
        return deviceGroupId.ToString().Substring(24, 12).ToLowerInvariant();
    }
}
