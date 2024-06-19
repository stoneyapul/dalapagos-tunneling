namespace Dalapagos.Tunneling.Core.Extensions;

using Model;

public static class ServerLocationExtensions
{
    public static string ToAzureLocation(this ServerLocation location)
    {
        return location switch
        {
            ServerLocation.West => "westus3",
            ServerLocation.Central => "centralus",
            ServerLocation.East => "eastus2",
            _ => throw new ArgumentOutOfRangeException(nameof(location), location, null)
        };
    }
}