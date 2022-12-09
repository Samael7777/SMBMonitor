using NativeApi.Wifi.Base;
using NativeApi.Wifi.Win32.DTO;
using static NativeApi.Wifi.Win32.WifiAPI;

namespace NativeApi.Wifi;

public class WifiScanner
{
    public WifiScanner(Guid adapterId)
    {
        AdapterId = adapterId;
    }

    public Guid AdapterId { get; }

    public IEnumerable<WifiNetworkInfo> GetAvailableNetworks()
    {
        using var handle = OpenHandle();

        var networks = Win32.WifiAPI.GetAvailableNetworks(handle, AdapterId);
        return networks.Select(n => n.ToWifiNetworkInfo(AdapterId));
    }

    public WifiNetworkInfo? ConnectedNetwork()
    {
        var result = GetAvailableNetworks()
            .FirstOrDefault(n => n.Flags.Connected);

        return result;
    }

    public void Scan()
    {
        using var handle = OpenHandle();
        Win32.WifiAPI.Scan(handle, AdapterId);
    }
}