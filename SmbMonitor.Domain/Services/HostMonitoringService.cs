using System.Net;
using SmbMonitor.Base.DTO;
using SmbMonitor.Base.DTO.Nodes;
using SmbMonitor.Base.Extensions;
using SmbMonitor.Base.Interfaces;
using SmbMonitor.Base.Interfaces.Tools;

namespace SmbMonitor.Domain.Services;

public class HostMonitoringService : IHostMonitoringService
{
    private readonly IPortScanner _scanner;
    private readonly Settings _settings;

    public HostMonitoringService(IPortScanner scanner, Settings settings)
    {
        _scanner = scanner;
        _settings = settings;
    }

    public void DoPortScan(IEnumerable<HostNode> nodes)
    {
        Parallel.ForEach(nodes, ScanTask);
    }

    //todo Добавить обработку всего списка адресов ноды
    private void ScanTask(HostNode node)
    {
        if (node.IsScanning) return;

        var addressList = (IPAddress[]?)node.Host.AddressList;
        if (addressList is null || addressList.IsEmpty())
            return;
        
        node.SetScanning();
        var address = node.Host.AddressList[0];

        if (_scanner.IsPortOpen(address, _settings.SmbPort, _settings.ScanTimeoutMs))
            node.SetSmbServerConnectedResetTries();
        else
            node.SetSmbServerDisconnectedReachingTriesLimit(_settings.TriesToUnaccessible);
        
        node.SetNotScanning();
    }
}