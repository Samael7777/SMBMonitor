using System.Collections.Specialized;
using System.ComponentModel;
using NativeApi.NetworkUtils;
using NativeApi.Wifi.Base;
using SmbMonitorLib.Services.Base;

namespace SmbMonitorLib.Interfaces;

public interface IStorageService
{
    IReadOnlyDictionary<MonitorNode, MonitorNodeInfo> MonitorNodes { get; }
    event PropertyChangedEventHandler? StorageItemChanged;
    event NotifyCollectionChangedEventHandler? StorageCollectionChanged;
    void AddSsid(WifiSSID ssid, Credentials credentials);
    void AddHost(Host host, Credentials credentials);
    void AddItem(MonitorNode item);
    void RemoveItem(MonitorNode item);
    internal void AddExternalHost(Host host);
}