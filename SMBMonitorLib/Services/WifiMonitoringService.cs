using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using NativeApi.NetworkUtils;
using NativeApi.Wifi;
using NativeApi.Wifi.Base;
using SmbMonitorLib.Interfaces;
using SmbMonitorLib.Services.Base;

namespace SmbMonitorLib.Services;

internal class WifiMonitoringService : ControlledService<WifiMonitoringService>, IWifiMonitoringService
{
    private readonly WifiEventsNotificator _eventsNotificator;
    private readonly IStorageService _storageService;

    public WifiMonitoringService(IStorageService storageService, ISettings settings)
    {
        Logger = settings.Logger;
        _eventsNotificator = new WifiEventsNotificator();
        _eventsNotificator.OnAdapterStateChanged += AdapterStateChanged;
       
        _storageService = storageService;
    }

    protected override void OnStart()
    {
        CheckMonitorsList(_storageService.MonitorNodes.Keys);
        _eventsNotificator.StartWifiNotifications();
        _storageService.StorageCollectionChanged += OnStorageChanged;
    }

    protected override void OnStop()
    {
        _storageService.StorageCollectionChanged -= OnStorageChanged;
        _eventsNotificator.StopWifiNotifications();
    }

    private void OnStorageChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CheckMonitorsList(e.NewItems);
    }

    private void AdapterStateChanged(Guid adapter, AdapterState state)
    {
        switch (state)
        {
            case AdapterState.Unknown:
                break;
            case AdapterState.Connected:
                OnAPConnected(adapter);
                break;
            case AdapterState.Disconnected:
                OnAPDisconnected(adapter);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    private void CheckMonitorsList(IEnumerable? list)
    {
        if (list is null) return;
        Parallel.ForEach(list.ToList<MonitorNode>(), CheckAndProcessAPConnection);
    }

    private void CheckAndProcessAPConnection(MonitorNode node)
    {
        var ap = node.WifiSSID;
        var adapterId = WifiAdapters.GetAdapterByConnectedNetwork(ap.SSIDBytes)?.Id;
        if (adapterId is { } id)
            ProcessConnectedAP(node, id);
    }

    private void OnAPConnected(Guid adapterId)
    {
        var connectedAP = WifiAdapters.GetConnectedNetwork(adapterId);
        if (connectedAP == WifiSSID.Empty) return;

        var node = _storageService.MonitorNodes.Keys
            .FirstOrDefault(item => item.Type == NodeType.AP && item.WifiSSID == connectedAP);
        if (node is null) return;

        ProcessConnectedAP(node, adapterId);
    }

    private void OnAPDisconnected(Guid adapterId)
    {
        var disconnectedNodes = _storageService.MonitorNodes
            .Where(node => 
                node.Key.Type == NodeType.AP 
                && node.Value.IsApConnected 
                && node.Value.AdapterId.Equals(adapterId));

        foreach (var node in disconnectedNodes) 
            ProcessDisconnectedAP(node.Key);
    }

    private void ProcessConnectedAP(MonitorNode node, Guid adapterId)
    {
        var value = _storageService.MonitorNodes[node];
        var ap = node.WifiSSID;
        var address = GetAccessPointAddress(adapterId);

        LogWriteLine($"Точка доступа {ap} доступна, IP адрес {address}.");

        value.AdapterId = adapterId;
        node.Host = new Host(address);

        value.IsApConnected = true;
    }

    private void ProcessDisconnectedAP(MonitorNode node)
    {
        var ap = node.WifiSSID;
        var value = _storageService.MonitorNodes[node];
        value.AdapterId = Guid.Empty;

        LogWriteLine($"Точка доступа {ap} отключена.");
        
        value.IsApConnected = false;
        value.SmbServerStatus = SmbServerStatus.Disconnected;
    }

    private static IPAddress GetAccessPointAddress(Guid adapterId)
    {
        var adapter = GetAdapterByGuid(adapterId);
        if (adapter == null) return IPAddress.None;

        return adapter.GetIPProperties().GatewayAddresses
                   .FirstOrDefault(u =>
                       u.Address.AddressFamily == AddressFamily.InterNetwork)?.Address
               ?? IPAddress.None;
    }

    private static NetworkInterface? GetAdapterByGuid(Guid adapterId)
    {
        if (adapterId.Equals(Guid.Empty)) return null;

        return NetworkInterface.GetAllNetworkInterfaces()
            .FirstOrDefault(i => Guid.Parse(i.Id) == adapterId);
    }
}