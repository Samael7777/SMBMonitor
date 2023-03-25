using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using SmbMonitor.Base.Interfaces;
using SmbMonitor.Base.DTO.Nodes;
using SmbMonitor.Base.Exceptions.Storage;


namespace SmbMonitor.Domain.Services;

public class MemoryStorage : IStorage
{
    private readonly ConcurrentDictionary<HostNode, object?> _monitorNodes;

    public event PropertyChangedEventHandler? StorageItemChanged;
    public event NotifyCollectionChangedEventHandler? StorageCollectionChanged;

    public MemoryStorage()
    {
        _monitorNodes = new ConcurrentDictionary<HostNode, object?>();
    }
    
    public IReadOnlyCollection<HostNode> MonitorNodes => _monitorNodes.Keys.ToImmutableList();

    public void AddItem(HostNode item)
    {
        if (_monitorNodes.ContainsKey(item))
            throw new ItemAlreadyExistException(item);

        if (!_monitorNodes.TryAdd(item, null))
            throw new StorageException();

        item.PropertyChanged += ItemChangedEventHandler;

        OnStorageChanged(NotifyCollectionChangedAction.Add, item);
    }

    public void RemoveItem(HostNode item)
    {
        if (!_monitorNodes.ContainsKey(item))
            throw new ItemNotExistException(item);

        item.PropertyChanged -= ItemChangedEventHandler;

        if (!_monitorNodes.TryRemove(item, out _))
            throw new StorageException();
        
        OnStorageChanged(NotifyCollectionChangedAction.Remove, item);
    }

    private void OnStorageChanged(NotifyCollectionChangedAction action, object? items)
    {
        var args = items switch
        {
            HostNode node => new NotifyCollectionChangedEventArgs(action, node),
            IEnumerable<HostNode> enumerable => new NotifyCollectionChangedEventArgs(action, enumerable.ToImmutableList()),
            _ => throw new ArgumentException(null, nameof(items))
        };

        StorageCollectionChanged?.Invoke(MonitorNodes, args);
    }
    
    private void ItemChangedEventHandler(object? sender, PropertyChangedEventArgs args)
    {
        StorageItemChanged?.Invoke(sender, args);
    }
}