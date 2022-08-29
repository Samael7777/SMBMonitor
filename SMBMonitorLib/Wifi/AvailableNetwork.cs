using SmbMonitorLib.Wifi.Win32;

namespace SmbMonitorLib.Wifi;

/// <summary>
///     Wireless LAN information on available network
/// </summary>
public class AvailableNetwork
{
    internal AvailableNetwork(WlanAvailableNetwork wlan, InterfaceInfo interfaceInfo)
    {

        Interface = interfaceInfo;

        Ssid = new byte[wlan.dot11Ssid.SsidLength];
        if (wlan.dot11Ssid.SsidLength != 0)
            Array.Copy(wlan.dot11Ssid.Ssid, Ssid, wlan.dot11Ssid.SsidLength);

        BssType = BssTypeConverter.Convert(wlan.dot11BssType);
        SignalQuality = (int)wlan.wlanSignalQuality;
        IsSecurityEnabled = wlan.SecurityEnabled;
        ProfileName = wlan.ProfileName;
        AuthenticationAlgorithm = AuthenticationAlgorithmConverter.Convert(wlan.dot11DefaultAuthAlgorithm);
        CipherAlgorithm = CipherAlgorithmConverter.Convert(wlan.dot11DefaultCipherAlgorithm);
        Flags = new ConnectionFlags(wlan.Flags);
    }

    /// <summary>
    ///     Associated wireless interface information
    /// </summary>
    public InterfaceInfo Interface { get; }

    /// <summary>
    ///     SSID (maximum 32 bytes)
    /// </summary>
    public byte[] Ssid { get; }

    /// <summary>
    ///     BSS network type
    /// </summary>
    public BssType BssType { get; }

    /// <summary>
    ///     Signal quality (0-100)
    /// </summary>
    public int SignalQuality { get; }

    /// <summary>
    ///     Whether security is enabled on this network
    /// </summary>
    public bool IsSecurityEnabled { get; }

    /// <summary>
    ///     Associated wireless profile name
    /// </summary>
    public string ProfileName { get; }

    /// <summary>
    ///     Default authentication algorithm to be used to connect to this network for the first time
    /// </summary>
    public AuthenticationAlgorithm AuthenticationAlgorithm { get; }

    /// <summary>
    ///     Default cipher algorithm to be used to connect to this network
    /// </summary>
    public CipherAlgorithm CipherAlgorithm { get; }

    /// <summary>
    ///     Connection flags according
    /// </summary>
    public ConnectionFlags Flags { get; }
}