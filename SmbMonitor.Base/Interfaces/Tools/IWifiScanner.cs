using SmbMonitor.Base.DTO.Wifi;

namespace SmbMonitor.Base.Interfaces.Tools;

public interface IWifiScanner
{
    Guid AdapterId { get; }
    IEnumerable<WifiNetworkInfo> GetAvailableNetworks();
    WifiNetworkInfo? ConnectedNetwork();
}