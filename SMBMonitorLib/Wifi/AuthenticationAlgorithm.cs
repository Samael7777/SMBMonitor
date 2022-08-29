using SmbMonitorLib.Wifi.Win32;

namespace SmbMonitorLib.Wifi;

/// <summary>
///     Authentication algorithm
/// </summary>
/// <remarks>
///     Equivalent to DOT11_AUTH_ALGORITHM:
///     https://docs.microsoft.com/en-us/windows/win32/nativewifi/dot11-auth-algorithm
/// </remarks>
public enum AuthenticationAlgorithm
{
    /// <summary>
    ///     Unknown (invalid value)
    /// </summary>
    Unknown = 0,

    /// <summary>
    ///     802.11 Open System authentication algorithm
    /// </summary>
    Open,

    /// <summary>
    ///     802.11 Shared Key authentication algorithm that uses pre-shared Wired Equivalent Privacy (WEP) key
    /// </summary>
    Shared,

    /// <summary>
    ///     Wi-Fi Protected Access (WPA) algorithm
    /// </summary>
    Wpa,

    /// <summary>
    ///     WPA algorithm that uses pre-shared keys (PSK)
    /// </summary>
    WpaPsk,

    /// <summary>
    ///     Note supported
    /// </summary>
    WpaNone,

    /// <summary>
    ///     802.11i Robust Security Network Association (RSNA) algorithm (WPA2 is one such algorithm.)
    /// </summary>
    Rsna,

    /// <summary>
    ///     802.11i RSNA algorithm that uses PSK
    /// </summary>
    RsnaPsk,

    /// <summary>
    ///     WPA3 algorithm
    /// </summary>
    Wpa3,

    /// <summary>
    ///     WPA3 Simultaneous Authentication of Equals (SAE）algorithm
    /// </summary>
    Wpa3Sae,

    /// <summary>
    ///     Opportunistic Wireless Encryption (OWE) algorithm
    /// </summary>
    Owe,

    /// <summary>
    ///     Indicates the start of the range that specifies proprietary authentication algorithms developed by an independent
    ///     hardware vendor (IHV).
    /// </summary>
    IhvStart,

    /// <summary>
    ///     Indicates the end of the range that specifies proprietary authentication algorithms developed by an independent
    ///     hardware vendor (IHV).
    /// </summary>
    IhvEnd
}

internal static class AuthenticationAlgorithmConverter
{
    public static AuthenticationAlgorithm Convert(Dot11AuthAlgorithm source)
    {
        return source switch
        {
            Dot11AuthAlgorithm.Dot11AuthAlgo80211Open => AuthenticationAlgorithm.Open,
            Dot11AuthAlgorithm.Dot11AuthAlgo80211SharedKey => AuthenticationAlgorithm.Shared,
            Dot11AuthAlgorithm.Dot11AuthAlgoWpa => AuthenticationAlgorithm.Wpa,
            Dot11AuthAlgorithm.Dot11AuthAlgoWpaPsk => AuthenticationAlgorithm.WpaPsk,
            Dot11AuthAlgorithm.Dot11AuthAlgoWpaNone => AuthenticationAlgorithm.WpaNone,
            Dot11AuthAlgorithm.Dot11AuthAlgoRsna => AuthenticationAlgorithm.Rsna,
            Dot11AuthAlgorithm.Dot11AuthAlgoRsnaPsk => AuthenticationAlgorithm.RsnaPsk,
            Dot11AuthAlgorithm.Dot11AuthAlgoWpa3 => AuthenticationAlgorithm.Wpa3,
            Dot11AuthAlgorithm.Dot11AuthAlgoWpa3Sae => AuthenticationAlgorithm.Wpa3Sae,
            Dot11AuthAlgorithm.Dot11AuthAlgoOwe => AuthenticationAlgorithm.Owe,
            Dot11AuthAlgorithm.Dot11AuthAlgoIhvStart => AuthenticationAlgorithm.IhvStart,
            Dot11AuthAlgorithm.Dot11AuthAlgoIhvEnd => AuthenticationAlgorithm.IhvEnd,
            _ => AuthenticationAlgorithm.Unknown
        };
    }
}