using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NUnit.Framework;
using SmbMonitor.Domain.Tools;

namespace SMBMonitor.Domain.Tests.Tools;

//todo протестировать граничные условия входных параметров сканера

[TestFixture]
public class PortScannerTests
{
    private readonly IPAddress TestAddress = IPAddress.Loopback;
    private const int TestPort = 12345;
    private const int ListenerTimeout = 2000;
    private const int ScanTimeout = ListenerTimeout / 2;

    private readonly TcpListener _tcpListener;
    private readonly PortScanner _portScanner;
    private Task _listenerTask = null!;

    public PortScannerTests()
    {
        _tcpListener = new TcpListener(TestAddress, TestPort);
        _portScanner = new PortScanner();
    }

    [SetUp]
    public void SetUp()
    {
        _listenerTask = GetNewListenerTask();
        _listenerTask.Start();
    }

    [TearDown]
    public void TearDown()
    {
        _tcpListener.Stop();
    }

    [Test]
    public void ScanOpenPortTest()
    {
        var result = _portScanner.IsPortOpen(TestAddress, TestPort, ScanTimeout);
        _listenerTask.Wait(ListenerTimeout);

        Assert.AreEqual(true, result);
    }

    [Test]
    public void ScanClosedPortTest()
    {
        _tcpListener.Stop();

        var result = _portScanner.IsPortOpen(TestAddress, TestPort, ScanTimeout);
        
        Assert.AreEqual(false, result);
    }

    [Test]
    public async Task ScanOpenPortTestAsync()
    {
        var result = await _portScanner.IsPortOpenAsync(TestAddress, TestPort, ScanTimeout);
        Assert.AreEqual(true, result);
    }

    [Test]
    public async Task ScanClosedPortTestAsync()
    {
        _tcpListener.Stop();
        var result = await _portScanner.IsPortOpenAsync(TestAddress, TestPort, ScanTimeout);
        Assert.AreEqual(false, result);
    }

    private Task GetNewListenerTask()
    {
        return new Task(() =>
        {
            _tcpListener.Start();
            try
            {
                _tcpListener.AcceptSocket();
            }
            catch (SocketException ex) when (ex.ErrorCode == 10004)
            { }
        });
    }
}
