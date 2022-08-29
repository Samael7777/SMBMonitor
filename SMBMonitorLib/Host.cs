using System.Net;
using System.Net.Sockets;

namespace SmbMonitorLib;

public class Host : IEquatable<Host>
{
    private readonly IPAddress _address;
    private readonly string _hostName;

    public Host()
    {
        _address = IPAddress.None;
        _hostName = string.Empty;
    }

    public Host(string hostName, int port)
    {
        _hostName = hostName;
        _address = IPAddress.None;
        Port = port;
    }

    public Host(IPAddress address, int port)
    {
        _address = address;
        _hostName = string.Empty;
        Port = port;
    }

    /// <summary>
    ///     IP отслеживаемого хоста
    /// </summary>
    public IPAddress IP => GetAddress();

    /// <summary>
    ///     Имя отслеживаемого хоста
    /// </summary>
    public string Name => GetHostname();
   
    /// <summary>
    ///     Отслеживаемый порт
    /// </summary>
    public int Port { get; set; }

    #region Equals
    public bool Equals(Host? other)
    {
        if (other is null) return false;
        return ReferenceEquals(this, other) 
               || GetHashCode().Equals(other.GetHashCode());
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Host)obj);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
    
    #endregion
    
    private IPAddress GetAddress()
    {
        if (!_address.Equals(IPAddress.None))
            return _address;

        IPAddress address;
        try
        {
            var list = Dns.GetHostAddresses(_hostName);
            address = list.FirstOrDefault
                    (ip=>ip.AddressFamily == AddressFamily.InterNetwork)
                ?? IPAddress.None;
        }
        catch (SocketException)
        {
            address = IPAddress.None;
        }
        return address;
    }

    private string GetHostname()
    {
        string hostname;
        try
        {
            hostname = Dns.GetHostEntry(_address).HostName;
        }
        catch (SocketException)
        {
            hostname = _address.Equals(IPAddress.None)
                ? _hostName
                : _address.ToString();
        }
        return hostname;
    }
}
