using SmbMonitor.Base.DTO.Wifi;
using SmbMonitor.NativeApi.Wifi.Win32.DTO;

namespace SmbMonitor.NativeApi.Wifi.Extensions;

internal static class WlanAvailableNetworkExtensions
{
    public static WifiNetworkInfo ToWifiNetworkInfo(this WlanAvailableNetwork network, Guid adapterId)
    {
        return new WifiNetworkInfo
        {
            AssociatedAdapterId = adapterId,
            Ssid = new WifiSSID(GetSsid(network)),
            BssType = network.dot11BssType.ToBssType(),
            SignalQuality = (int)network.wlanSignalQuality,
            IsSecurityEnabled = network.SecurityEnabled,
            ProfileName = network.ProfileName,
            AuthenticationAlgorithm = network.dot11DefaultAuthAlgorithm.Convert(),
            CipherAlgorithm = network.dot11DefaultCipherAlgorithm.ToCipherAlgorithm(),
            Flags = ConnectionFlags.FromBitMask(network.Flags),
        };
    }

    private static byte[] GetSsid(WlanAvailableNetwork wlan)
    {
        if (wlan.dot11Ssid.Ssid is null) return Array.Empty<byte>();

        var ssid = new byte[wlan.dot11Ssid.SsidLength];
        if (wlan.dot11Ssid.SsidLength != 0)
            Array.Copy(wlan.dot11Ssid.Ssid, ssid, wlan.dot11Ssid.SsidLength);
        return ssid;
    }
}