using System.Collections.Specialized;
using SmbMonitor.Base.DTO.Nodes;
using SmbMonitor.Base.DTO.Wifi;
using SmbMonitor.Base.Extensions;
using SmbMonitor.Base.Interfaces;
using SmbMonitor.Base.Interfaces.Tools;

namespace SmbMonitor.Domain.Services;

public class WifiMonitoringService : IWifiMonitoringService
{
    private readonly IStorage _storage;
    private readonly IWifiClient _wifiClient;

    public WifiMonitoringService(IWifiClient wifiClient, IStorage storage)
    {
        _wifiClient = wifiClient;
        _storage = storage;
    }

    public void OnAdapterStateChanged(Guid adapter, AdapterState state)
    {
        switch (state)
        {
            case AdapterState.Connected:
                OnAPConnected(adapter);
                break;
            case AdapterState.Disconnected:
                OnAPDisconnected(adapter);
                break;
            case AdapterState.Unknown:
            default:
                break;
        }
    }

    public void OnStorageChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is null) return;
        foreach (var item in e.NewItems)
        {
            if (item is not WifiNode wifiNode) continue;
            CheckNodeForWifiConnection(wifiNode);
        }
    }

    private void OnAPConnected(Guid adapterId)
    {
        var connectedAP = _wifiClient.GetConnectedNetwork(adapterId);
        if (connectedAP.IsEmpty) return;

        var node = GetWifiNodeFromStorage(connectedAP);
        if (node is null) return;

        SetWifiNodeApConnected(node, adapterId);
    }

    private void OnAPDisconnected(Guid adapterId)
    {
        var nodesToDisconnect = GetWifiNodesFromStorage(adapterId);
        nodesToDisconnect.ForEach(n=>n.SetWifiApSmbServerDisconnected());
    }

    private void SetWifiNodeApConnected(WifiNode node, Guid adapterId)
    {
        var address = _wifiClient.GetConnectedAccessPointAddress(adapterId);
        node.SetWifiApConnected(adapterId, address);
    }
    
    private WifiNode? GetWifiNodeFromStorage(WifiSSID ssid)
    {
        return _storage.MonitorNodes
            .FirstOrDefault(item => 
                item is WifiNode node && node.WifiSSID == ssid) 
            as WifiNode;
    }

    private IEnumerable<WifiNode> GetWifiNodesFromStorage(Guid adapterId)
    {
        return _storage.MonitorNodes.Where(node =>
                node is WifiNode { IsAPConnected: true } wifiNode
                && wifiNode.AdapterId == adapterId)
            .Cast<WifiNode>();
    }

    private void CheckNodeForWifiConnection(WifiNode node)
    {
        var adapterId = _wifiClient.GetAdapterWithConnectedSsid(node.WifiSSID).Id;
        if (adapterId == Guid.Empty) return; 
            
        SetWifiNodeApConnected(node, adapterId);
    }
}