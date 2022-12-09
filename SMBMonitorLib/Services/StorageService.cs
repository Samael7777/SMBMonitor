using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using NativeApi.NetworkUtils;
using NativeApi.Wifi.Base;
using SmbMonitorLib.Exceptions;
using SmbMonitorLib.Interfaces;
using SmbMonitorLib.Services.Base;


namespace SmbMonitorLib.Services;

internal class StorageService : BaseService<StorageService>, IStorageService
{
    private readonly ConcurrentDictionary<MonitorNode, MonitorNodeInfo> _monitorNodes;

    public StorageService(ISettings settings)
    {
        _monitorNodes = new ConcurrentDictionary<MonitorNode, MonitorNodeInfo>();
        Logger = settings.Logger;
    }

    public event PropertyChangedEventHandler? StorageItemChanged;
    public event NotifyCollectionChangedEventHandler? StorageCollectionChanged;

    public IReadOnlyDictionary<MonitorNode, MonitorNodeInfo> MonitorNodes => 
        new ReadOnlyDictionary<MonitorNode, MonitorNodeInfo>(_monitorNodes);

    public void AddSsid(WifiSSID ssid, Credentials credentials)
    {
        var node = new MonitorNode
        {
            Type = NodeType.AP,
            WifiSSID = ssid,
            Description = ssid.ToString(),
            Credentials = credentials
        };
        AddItem(node);
    }

    public void AddHost(Host host, Credentials credentials)
    {
        var node = new MonitorNode
        {
            Type = NodeType.Host,
            Host = host,
            Description = host.Description,
            Credentials = credentials
        };
        RemoveExternalWithSameHost(host);
        AddItem(node);
    }

    public void AddItem(MonitorNode item)
    {
        if (_monitorNodes.ContainsKey(item))
            throw new StorageException("Item already exists.");
        
        var info = new MonitorNodeInfo(item);

        if (!_monitorNodes.TryAdd(item, info))
            throw new StorageException("Storage error");

        info.PropertyChanged += ItemChangedEventHandler;
        
        LogWriteLine($"Добавлен мониторинг {GetTypeMessage(item.Type)} {item.Description}");

        OnStorageChanged(NotifyCollectionChangedAction.Add, item);
    }

    public void RemoveItem(MonitorNode item)
    {
        if (!_monitorNodes.ContainsKey(item))
            throw new StorageException("Item not exists.");

        _monitorNodes[item].PropertyChanged -= ItemChangedEventHandler;

        if(!_monitorNodes.TryRemove(item, out _))
            throw new StorageException("Storage error");

        LogWriteLine($"Удален мониторинг {GetTypeMessage(item.Type)} {item.Description}");

        OnStorageChanged(NotifyCollectionChangedAction.Remove, item);
    }

    private void RemoveExternalWithSameHost(Host host)
    {
        var externalToDelete = _monitorNodes
            .Where(n => n.Key.Type == NodeType.ExternalHost && n.Key.Host == host);
        
        foreach (var item in externalToDelete)
        {
            RemoveItem(item.Key);
        }
    }

    private void OnStorageChanged(NotifyCollectionChangedAction action, MonitorNode item)
    {
        var args = new NotifyCollectionChangedEventArgs(action, item);
        StorageCollectionChanged?.Invoke(MonitorNodes, args);
    }

    void IStorageService.AddExternalHost(Host host)
    {
        var node = new MonitorNode
        {
            Type = NodeType.ExternalHost,
            Host = host,
            Description = host.Description
        };

        AddItem(node);
    }

    private void ItemChangedEventHandler(object? sender, PropertyChangedEventArgs args)
    {
        if (sender is not MonitorNodeInfo info) return;
        RemoveExternalWithSameHost(info.LinkedNode.Host);
        StorageItemChanged?.Invoke(sender, args);
    }

    private string GetTypeMessage(NodeType type)
    {
        return type switch
        {
            NodeType.Unknown => "неизвестного типа",
            NodeType.AP => "точки доступа",
            NodeType.Host => "хоста",
            NodeType.ExternalHost => "внешнего хоста",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}