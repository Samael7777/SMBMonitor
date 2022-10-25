using System.Net;
using System.Net.Sockets;

namespace NetworkUtils;

public class IPv4Resolver
{
    private readonly string _hostname;

    public IPv4Resolver(string hostname)
    {
        if (hostname == string.Empty)
            throw new ArgumentNullException(nameof(IPv4Resolver));

        _hostname = hostname;
    }

    public IPAddress GetAddress()
    {
        if (IPAddress.TryParse(_hostname, out var address))
            return address;

        try
        {
            address = Dns.GetHostAddresses(_hostname)
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