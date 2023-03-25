using SmbMonitor.Base.Interfaces.Tools;
using System.Collections.Specialized;
using System.ComponentModel;

namespace SmbMonitor.Base.Interfaces;

public interface ISmbMonitoringService
{
    void OnNodeStatusChanged(object? sender, PropertyChangedEventArgs e);
    void OnNodeListChanged(object? sender, NotifyCollectionChangedEventArgs e);
}