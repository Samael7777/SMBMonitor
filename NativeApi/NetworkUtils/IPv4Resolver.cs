using System.Net;
using System.Net.Sockets;

namespace NetworkUtils;

public static class IPv4Resolver
{
    public static IPAddress GetAddress(string hostname)
    {
        if (IPAddress.TryParse(hostname, out var address))
            return address;

        try
        {
            address = Dns.GetHostAddresses(hostname)
                          .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                      ?? IPAddress.None;
        }
        catch (SocketException)
        {
            address = IPAddress.None;
        }

        return address;
    }
}