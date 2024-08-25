namespace Dalapagos.Tunneling.Rport.Client;

using Core.Model;

internal class TunnelMapper
{
    public static Tunnel? Map(RportTunnelData source, string baseAddress)
    {
        var remotePort = source.RemotePort ?? throw new Exception("Remote port is missing.");
        var localPort = source.LocalPort ?? throw new Exception("Local port is missing.");

        if (!Enum.TryParse(source.Scheme, true, out Protocol protocol))
        {
            switch (source.RemotePort)
            {
                case "22":
                    protocol = Protocol.Ssh;
                    break;
                case "443":
                    protocol = Protocol.Https;
                    break;
                default:
                    return null;
            }
        }

        var url = source.TunnelUrl;
        if (string.IsNullOrWhiteSpace(source.TunnelUrl))
        {
            var domain = new Uri($"https://{baseAddress}");

            switch (protocol)
            {
                case Protocol.Https:
                    url = $"https://{domain.Host}:{localPort}";
                    break;
                case Protocol.Ssh:
                    url = $"ssh://<username>@{domain.Host}:{localPort}";
                    break;
                default:
                    break;
            }
        }

        return new Tunnel(source.TunnelId, ushort.Parse(remotePort), ushort.Parse(localPort))
        {
            Protocol = protocol,
            Url = url,
            AllowedIps = !string.IsNullOrWhiteSpace(source.Acl) ? source.Acl.Split(',') : [],
        };
    }
}
