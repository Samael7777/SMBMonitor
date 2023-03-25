using System.Net;
using NUnit.Framework;
using SmbMonitor.Domain.Comparers;
using SMBMonitor.Domain.Tests.Fakes;

namespace SMBMonitor.Domain.Tests.Comparers;

[TestFixture]
public class HostsComparerTests
{
    private readonly HostsComparer _hostComparer;

    public HostsComparerTests()
    {
        var dnsResolver = new DnsResolverStub();
        _hostComparer = new HostsComparer(dnsResolver);
    }

    [Test]
    public void CompareHostsWithSameHostnames()
    {
        var host1 = new IPHostEntry { HostName = "host" };
        var host2 = new IPHostEntry { HostName = "HOST" };

        Assert.AreEqual(true, _hostComparer.Equals(host1, host2));
    }

    [Test]
    public void CompareHostsWithSameIPs()
    {
        var host1 = new IPHostEntry { AddressList = new[] { IPAddress.Parse("10.10.10.10") } };
        var host2 = new IPHostEntry { AddressList = new[] { IPAddress.Parse("10.10.10.10") } };

        Assert.AreEqual(true, _hostComparer.Equals(host1, host2));
    }

    [Test]
    public void CompareSameHostsWithIPAndHostname()
    {
        var host1 = new IPHostEntry { HostName = "test1" };
        var host2 = new IPHostEntry { AddressList = new[] { IPAddress.Parse("10.10.10.10") } };

        Assert.AreEqual(true, _hostComparer.Equals(host1, host2));
    }

    public void CompareHostsWithDiffHostnames()
    {
        var host1 = new IPHostEntry { HostName = "host1" };
        var host2 = new IPHostEntry { HostName = "HOST2" };

        Assert.AreEqual(false, _hostComparer.Equals(host1, host2));
    }

    [Test]
    public void CompareHostsWithDiffIPs()
    {
        var host1 = new IPHostEntry { AddressList = new[] { IPAddress.Parse("10.10.10.10") } };
        var host2 = new IPHostEntry { AddressList = new[] { IPAddress.Parse("10.10.10.20") } };

        Assert.AreEqual(false, _hostComparer.Equals(host1, host2));
    }

    [Test]
    public void CompareDiffHostsWithIPAndHostname()
    {
        var host1 = new IPHostEntry { HostName = "test2" };
        var host2 = new IPHostEntry { AddressList = new[] { IPAddress.Parse("10.10.10.10") } };

        Assert.AreEqual(false, _hostComparer.Equals(host1, host2));
    }
}