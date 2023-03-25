using System.Collections.Specialized;
using System.ComponentModel;
using SmbMonitor.Base.DTO.Nodes;

namespace SmbMonitor.Base.Interfaces;

public interface IStorage
{
    event PropertyChangedEventHandler? StorageItemChanged;
    event NotifyCollectionChangedEventHandler? StorageCollectionChanged;

    IReadOnlyCollection<HostNode> MonitorNodes { get; }
    void AddItem(HostNode item);
    void RemoveItem(HostNode item);
}