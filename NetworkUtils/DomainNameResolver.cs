using System.Net;
using System.Net.Sockets;

namespace NetworkUtils;

public class DomainNameResolver
{
    private readonly IPAddress _address;

    public DomainNameResolver(IPAddress address)
    {
        if (address.Equals(IPAddress.None))
            throw new ArgumentNullException(nameof(DomainNameResolver));

        _address = address;
    }

    public string GetDomainNameOrIP()
    {
        string domainName;
        try
        {
            domainName = Dns.GetHostEntry(_address).HostName;
        }
        catch (SocketException)
        {
            domainName = _address.ToString();
        }

        return domainName;
    }
}