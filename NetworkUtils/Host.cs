using System.Net;
using System.Net.Sockets;

namespace NetworkUtils;

public readonly struct Host : IEquatable<Host>
{
    private readonly string _hostName;
    private readonly IPAddress _hostIP;

    public Host(string hostName)
    {
        _hostName = hostName;
        _hostIP = IPAddress.None;
    }

    public Host(IPAddress address)
    {
        CheckAddress(address);
        _hostIP = address;
        _hostName = "";
    }

    public IPAddress IPAddress => GetAddress();

    public string DomainName => GetDomainName();

    private IPAddress GetAddress()
    {
        return _hostIP ?? new IPv4Resolver(DomainName).GetAddress();
    }

    private string GetDomainName()
    {
        return _hostName ?? new DomainNameResolver(IPAddress).GetDomainNameOrIP();
    }

    private static void CheckAddress(IPAddress address)
    {
        if (IsAddressIncorrect(address))
            throw new ArgumentException("Address must be correct IPv4 individual address");
    }

    private static bool IsAddressIncorrect(IPAddress address)
    {
        return address.AddressFamily != AddressFamily.InterNetwork
               || address.Equals(IPAddress.None)
               || address.Equals(IPAddress.Any)
               || address.Equals(IPAddress.Broadcast);
    }
    
    #region Equals

    public bool Equals(Host other)
    {
        if (_hostIP == null)
            return _hostName != null && _hostName.Equals(other.DomainName, StringComparison.OrdinalIgnoreCase);
        return _hostIP.Equals(other.IPAddress);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        return obj.GetType() == GetType() && Equals((Host)obj);
    }

    public override int GetHashCode()
    {
        return IPAddress.GetHashCode();
    }

    public static bool operator ==(Host left, Host right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Host left, Host right)
    {
        return !(left == right);
    }

    #endregion
}