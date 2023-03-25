using System.Net;
using System.Net.Sockets;
using SmbMonitor.Base.Interfaces.Tools;

namespace SmbMonitor.Domain.Tools;

public class SimpleDnsResolver : IDnsResolver
{
    private readonly Action<string, Exception>? _errorCallback;

    public SimpleDnsResolver(Action<string, Exception>? errorCallback = null)
    {
        _errorCallback = errorCallback;
    }

    public IPAddress[] GetIpV4Addresses(string host)
    {
        if (string.IsNullOrEmpty(host)) return Array.Empty<IPAddress>();
        if (IPAddress.TryParse(host, out var address)) return new[] { address };
        
        IPAddress[] result;
        try
        {
            result = Dns.GetHostAddresses(host, AddressFamily.InterNetwork);
        }
        catch(Exception ex)
        {
            result = Array.Empty<IPAddress>();
            _errorCallback?.Invoke(host, ex);
        }

        return result;
    }
}
