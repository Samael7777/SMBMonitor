using System.Collections.Concurrent;
using System.Net;
using SmbMonitor.Base.Exceptions.Storage;
using SmbMonitor.Base.Interfaces.Tools;

namespace SmbMonitor.Domain.Tools;

public class CachedDnsResolver : IDnsResolver
{
    private readonly ConcurrentDictionary<string, IPAddress[]> _dnsCache;
    private readonly IDnsResolver _resolver;

    public IReadOnlyDictionary<string, IPAddress[]> Records => _dnsCache;

    public CachedDnsResolver(IDnsResolver resolver)
    {
        _dnsCache = new ConcurrentDictionary<string, IPAddress[]>();
        _resolver = resolver;
    }

    public IPAddress[] GetIpV4Addresses(string host)
    {
        host = host.Trim().ToLower();

        if (_dnsCache.ContainsKey(host) && _dnsCache[host].Any())
            return _dnsCache[host];

        AddOrUpdateRecord(host, _resolver.GetIpV4Addresses(host));
        return _dnsCache[host];
    }

    public void UpdateRecords()
    {
        Parallel.ForEach(_dnsCache.Keys, host
            => AddOrUpdateRecord(host, _resolver.GetIpV4Addresses(host)));
    }

    public void AddOrUpdateRecord(string host, IPAddress[] newAddress)
    {
        host = host.Trim().ToLower();

        _dnsCache.AddOrUpdate(
            host,
            static (_, newAddr) => newAddr,
            static (_, oldAddr, newAddr)
                => newAddr.Any() ? newAddr : oldAddr,
            newAddress);
    }

    public void RemoveRecord(string host)
    {
        host = host.Trim().ToLower();

        if (!_dnsCache.ContainsKey(host))
            throw new ItemNotExistException(host);

        _dnsCache.Remove(host, out _);
    }

    public void FlushRecords()
    {
        _dnsCache.Clear();
    }
}