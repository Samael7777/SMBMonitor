using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Base;
using NetworkUtils;
using SmbMonitorLib.Exceptions;
using SmbMonitorLib.Services.Base;
using SmbMonitorLib.Services.DTO;
using Swordfish.NET.Collections;
using WifiAPI;
using WifiAPI.Base;

namespace SmbMonitorLib;

public class WifiMonitoringService : ControlledService<WifiMonitoringService>
{
    private static WifiMonitoringService? instance;
    private readonly WifiEventsNotificator _eventsNotificator = new();
    
    private WifiMonitoringService()
    {
        _eventsNotificator.OnAdapterConnected = OnAPConnected;
        _eventsNotificator.OnAdapterDisconnected = OnAPDisconnected;
        AccessPoints.CollectionChanged += AccessPointsCollectionChanged;
    }

    public static WifiMonitoringService Instance
    {
        get
        {
            if (instance == null)
                throw new InitializeException(nameof(Initialize));

            return instance;
        }
    }

    public static void Initialize()
    {
        if (instance != null) throw new AlreadyInitializedException();

        instance = new WifiMonitoringService();
    }


    public ConcurrentObservableDictionary<WifiSSID, WifiMonitoringData> AccessPoints { get; } = new();

    public event Action<Host, Credentials>? OnAccessPointConnected;
    public event Action<Host>? OnAccessPointDisconnected;

    public void AddAccessPoint(WifiSSID ap, Credentials credentials)
    {
        if (AccessPoints.ContainsKey(ap)) throw new ItemExistsException();

        var wmi = new WifiMonitoringData(credentials);
        AccessPoints.Add(ap, wmi);
    }

    public void RemoveAccessPoint(WifiSSID ap)
    {
        if (!AccessPoints.ContainsKey(ap)) throw new ItemNotExistsException();

        AccessPoints.Remove(ap);
    }

    private void AccessPointsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null) ProcessAddedItems(e.NewItems);
        if (e.OldItems != null) ProcessRemovedItems(e.OldItems);
    }

    private void ProcessAddedItems(IEnumerable list)
    {
        foreach (var item in list)
        {
            if (item is not KeyValuePair<WifiSSID, WifiMonitoringData> monitoringPair) continue;
            var ap = monitoringPair.Key;

            LogWriteLine($"Добавлена точка доступа {ap}");

            if (IsStarted) CheckAndProcessAPConnection(ap);
        }
    }

    private void ProcessRemovedItems(IEnumerable list)
    {
        foreach (var item in list)
        {
            if (item is not KeyValuePair<WifiSSID, WifiMonitoringData> monitoringPair) continue;
            var ap = monitoringPair.Key;

            LogWriteLine($"Удалена точка доступа {ap}");
        }
    }

    protected override void OnStart()
    {
        InitialCheck();
        _eventsNotificator.StartWifiNotifications();
    }

    protected override void OnStop()
    {
        _eventsNotificator.StopWifiNotifications();
    }

    private void InitialCheck()
    {
        Parallel.ForEach(AccessPoints.Keys, CheckAndProcessAPConnection);
    }

    private void CheckAndProcessAPConnection(WifiSSID ap)
    {
        var adapterId = WifiAdapters.GetAdapterByConnectedNetwork(ap.SSID)?.Id;
        if (adapterId is { } id)
            ProcessConnectedAP(ap, id);
    }

    private void OnAPConnected(Guid adapterId)
    {
        var ap = WifiAdapters.GetConnectedNetwork(adapterId);

        if (ap is not { } connectedAP) return;
        if (!AccessPoints.ContainsKey(connectedAP)) return;

        ProcessConnectedAP(connectedAP, adapterId);
    }

    private void OnAPDisconnected(Guid adapterId)
    {
        var disconnectedAPs = AccessPoints
            .Where(data => data.Value.IsConnected && data.Value.AdapterId.Equals(adapterId));

        foreach (var data in disconnectedAPs)
            ProcessDisconnectedAP(data.Key);
    }

    private void ProcessConnectedAP(WifiSSID ap, Guid adapterId)
    {
        if (!AccessPoints.ContainsKey(ap)) return;

        SetAPLastConnectionInfo(ap, adapterId);
        var address = AccessPoints[ap].IPAddress;
        AccessPoints[ap].IsConnected = true;

        LogWriteLine($"Точка доступа {ap} доступна, IP адрес {address}.");

        var host = new Host(address);
        var credentials = AccessPoints[ap].Credentials;

        OnAccessPointConnected?.Invoke(host, credentials);
    }

    private void ProcessDisconnectedAP(WifiSSID ap)
    {
        if (!AccessPoints.ContainsKey(ap)) return;

        var lastAddress = AccessPoints[ap].IPAddress;
        SetAPLastConnectionInfo(ap, Guid.Empty);
        AccessPoints[ap].IsConnected = false;

        LogWriteLine($"Точка доступа {ap} отключена.");

        OnAccessPointDisconnected?.Invoke(new Host(lastAddress));
    }

    private void SetAPLastConnectionInfo(WifiSSID ap, Guid adapterId)
    {
        AccessPoints[ap].AdapterId = adapterId;
        AccessPoints[ap].IPAddress = GetAccessPointAddress(adapterId);
    }

    private static IPAddress GetAccessPointAddress(Guid adapterId)
    {
        var adapter = GetAdapterByGuid(adapterId);
        if(adapter == null) return IPAddress.None;

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