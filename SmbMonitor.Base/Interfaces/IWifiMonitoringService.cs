using SmbMonitor.Base.Interfaces.Tools;
using System.Collections.Specialized;

namespace SmbMonitor.Base.Interfaces;

public interface IWifiMonitoringService
{
    void OnAdapterStateChanged(Guid adapter, AdapterState state);
    void OnStorageChanged(object? sender, NotifyCollectionChangedEventArgs e);
}