namespace Dalapagos.Tunneling.Core.Extensions;

using Model;

public static class EnumExtensions
{
   public static Protocol ToProtocol(this RestProtocol protocol)
    {
        return protocol switch
        {
            RestProtocol.Http => Protocol.Http,
            RestProtocol.Https => Protocol.Https,
            _ => throw new ArgumentOutOfRangeException(nameof(protocol), protocol, null)
        };
    }
}
