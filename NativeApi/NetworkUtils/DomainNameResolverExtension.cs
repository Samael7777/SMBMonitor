using System.Net;
using System.Net.Sockets;

namespace NativeApi.NetworkUtils;

public static class DomainNameResolverExtension
{
    public static string GetDomainNameOrIP(this IPAddress address)
    {
        if (address.Equals(IPAddress.None)) return string.Empty;

        string domainName;
        try
        {
            domainName = Dns.GetHostEntry(address).HostName;
        }
        catch (SocketException)
        {
            domainName = address.ToString();
        }

        return domainName;
    }
}