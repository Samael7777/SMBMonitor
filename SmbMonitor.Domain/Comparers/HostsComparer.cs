using System.Net;
using SmbMonitor.Base.Interfaces.Tools;

namespace SmbMonitor.Domain.Comparers;

public class HostsComparer : IEqualityComparer<IPHostEntry>
{
    private readonly IDnsResolver _dnsResolver;
    public HostsComparer(IDnsResolver dnsResolver)
    {
        _dnsResolver = dnsResolver;
    }

    public bool Equals(IPHostEntry? x, IPHostEntry? y)
    {
        if (ReferenceEquals(x, y)) return true;

        if (x is null || y is null) return false;
        if (x.GetType() != y.GetType()) return false;

        if (!string.IsNullOrWhiteSpace(x.HostName) && !string.IsNullOrWhiteSpace(y.HostName)
                && string.Equals(x.HostName, y.HostName, StringComparison.OrdinalIgnoreCase))
            return true;

        var task1 = GetAddressListTask(x);
        task1.Start();
        var task2 = GetAddressListTask(y);
        task2.Start();

        Task.WaitAll(task1, task2);

        var addrListX = task1.Result;
        var addrListY = task2.Result;

        return addrListX.Any(a => addrListY.Contains(a));
    }

    public int GetHashCode(IPHostEntry obj)
    {
        return obj.HostName.GetHashCode();
    }

    private Task<IPAddress[]> GetAddressListTask(IPHostEntry hostEntry)
    {
        return new Task<IPAddress[]>(() =>
        {
            var addrList = (IPAddress[]?)hostEntry.AddressList;
            if (addrList?.Any() ?? false)
                return addrList;
           
            return _dnsResolver.GetIpV4Addresses(hostEntry.HostName);
        });
    }
}
