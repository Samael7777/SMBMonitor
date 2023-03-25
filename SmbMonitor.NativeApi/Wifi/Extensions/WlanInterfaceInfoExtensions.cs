using SmbMonitor.Base.DTO.Wifi;
using SmbMonitor.NativeApi.Wifi.Win32.DTO;

namespace SmbMonitor.NativeApi.Wifi.Extensions;

internal static class WlanInterfaceInfoExtensions
{
    public static WifiAdapter ToWifiAdapter(this WlanInterfaceInfo net)
    {
        return new WifiAdapter(net.InterfaceGuid, net.InterfaceDescription, (InterfaceState)net.isState);
    }
}
