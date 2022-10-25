using WifiAPI.Win32;

namespace WifiAPI.Base;

public record struct WifiNetworkInfo
{
    internal WifiNetworkInfo(WlanAvailableNetwork wlan, Guid adapterId)
    {
        AssociatedAdapterId = adapterId;

        Id = new WifiSSID(GetSsid(wlan));

        BssType = BssTypeConverter.Convert(wlan.dot11BssType);
        SignalQuality = (int)wlan.wlanSignalQuality;
        IsSecurityEnabled = wlan.SecurityEnabled;
        ProfileName = wlan.ProfileName;
        AuthenticationAlgorithm = AuthenticationAlgorithmConverter.Convert(wlan.dot11DefaultAuthAlgorithm);
        CipherAlgorithm = CipherAlgorithmConverter.Convert(wlan.dot11DefaultCipherAlgorithm);
        Flags = new ConnectionFlags(wlan.Flags);
    }

    public Guid AssociatedAdapterId { get; }

    public WifiSSID Id { get; }

    public BssType BssType { get; }

    public int SignalQuality { get; }

    public bool IsSecurityEnabled { get; }

    public string ProfileName { get; }

    public AuthenticationAlgorithm AuthenticationAlgorithm { get; }

    public CipherAlgorithm CipherAlgorithm { get; }

    public ConnectionFlags Flags { get; }


    private static byte[] GetSsid(WlanAvailableNetwork wlan)
    {
        var ssid = new byte[wlan.dot11Ssid.SsidLength];
        if (wlan.dot11Ssid.SsidLength != 0)
            Array.Copy(wlan.dot11Ssid.Ssid, ssid, wlan.dot11Ssid.SsidLength);
        return ssid;
    }
}