﻿namespace SmbMonitor.Base.DTO.Wifi;

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