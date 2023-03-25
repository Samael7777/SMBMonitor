using System;
using System.Collections.Generic;
using System.Net;
using SmbMonitor.Base.Interfaces.Tools;

namespace SMBMonitor.Domain.Tests.Fakes;

internal class DnsResolverStub : IDnsResolver
{
    private readonly Dictionary<string, IPAddress> _fakeDnsRecords;
    public DnsResolverStub()
    {
        _fakeDnsRecords = new Dictionary<string, IPAddress>
        {
            { "test1", IPAddress.Parse("10.10.10.10") },
            { "test2", IPAddress.Parse("10.10.10.20") },
            { "test3", IPAddress.Parse("10.10.10.30") },
            { "test4.com", IPAddress.Parse("10.10.10.40") },
            { "www.test5.net", IPAddress.Parse("10.10.10.50") },
        };
    }
    public IPAddress[] GetIpV4Addresses(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
            return Array.Empty<IPAddress>();

        host = host.ToLower();

        if (IPAddress.TryParse(host, out var ip))
            return new[] { ip };

        return _fakeDnsRecords.ContainsKey(host) 
            ? new[] { _fakeDnsRecords[host] } 
            : Array.Empty<IPAddress>();
    }
}


