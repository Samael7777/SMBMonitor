using SmbMonitor.Base.DTO.Wifi;
using SmbMonitor.Base.Interfaces.Tools;
using SmbMonitor.NativeApi.Wifi.Extensions;
using SmbMonitor.NativeApi.Wifi.Win32;

namespace SmbMonitor.NativeApi.Wifi;

public class WifiScanner : IWifiScanner
{
    public WifiScanner(Guid adapterId)
    {
        AdapterId = adapterId;
    }

    public Guid AdapterId { get; }

    public IEnumerable<WifiNetworkInfo> GetAvailableNetworks()
    {
        using var handle = WifiAPI.OpenHandle();
        
        WifiAPI.Scan(handle, AdapterId);

        var networks = WifiAPI.GetAvailableNetworks(handle, AdapterId);
        return networks.Select(n => n.ToWifiNetworkInfo(AdapterId));
    }

    public WifiNetworkInfo? ConnectedNetwork()
    {
        var result = GetAvailableNetworks()
            .FirstOrDefault(n => n.Flags.Connected);

        return result;
    }
}