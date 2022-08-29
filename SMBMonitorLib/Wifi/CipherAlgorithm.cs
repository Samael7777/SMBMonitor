// ReSharper disable IdentifierTypo

using SmbMonitorLib.Wifi.Win32;

namespace SmbMonitorLib.Wifi;

/// <summary>
///     Cipher algorithm for data encryption and decryption
/// </summary>
/// <remarks>
///     Equivalent to DOT11_CIPHER_ALGORITHM:
///     https://docs.microsoft.com/en-us/windows/win32/nativewifi/dot11-cipher-algorithm
/// </remarks>
public enum CipherAlgorithm
{
    /// <summary>
    ///     No cipher algorithm is enabled or supported.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Wired Equivalent Privacy (WEP) algorithm with a cipher key of any length
    /// </summary>
    Wep,

    /// <summary>
    ///     WEP algorithm with a 40-bit cipher key
    /// </summary>
    Wep40,

    /// <summary>
    ///     WEP algorithm with a 104-bit cipher key
    /// </summary>
    Wep104,

    /// <summary>
    ///     Temporal Key Integrity Protocol (TKIP) algorithm
    /// </summary>
    Tkip,

    /// <summary>
    ///     AES-CCMP algorithm
    /// </summary>
    Ccmp,

    /// <summary>
    ///     Wi-Fi Protected Access (WPA) Use Group Key cipher suite
    /// </summary>
    WpaUseGroup,

    /// <summary>
    ///     Robust Security Network (RSN) Use Group Key cipher suite (not used)
    /// </summary>
    RsnUseGroup,

    /// <summary>
    ///     Indicates the start of the range that specifies proprietary cipher algorithms developed by an independent hardware
    ///     vendor (IHV).
    /// </summary>
    IhvStart,

    /// <summary>
    ///     Indicates the end of the range that specifies proprietary cipher algorithms developed by an independent hardware
    ///     vendor (IHV).
    /// </summary>
    IhvEnd,

    /// <summary>
    ///     Unknown algorithm
    /// </summary>
    Unknown = 0xFFFFFFF
}

internal static class CipherAlgorithmConverter
{
    public static CipherAlgorithm Convert(Dot11CipherAlgorithm source)
    {
        return source switch
        {
            Dot11CipherAlgorithm.Dot11CipherAlgoNone => CipherAlgorithm.None,
            Dot11CipherAlgorithm.Dot11CipherAlgoWep40 => CipherAlgorithm.Wep40,
            Dot11CipherAlgorithm.Dot11CipherAlgoTkip => CipherAlgorithm.Tkip,
            Dot11CipherAlgorithm.Dot11CipherAlgoCcmp => CipherAlgorithm.Ccmp,
            Dot11CipherAlgorithm.Dot11CipherAlgoWep104 => CipherAlgorithm.Wep104,
            Dot11CipherAlgorithm.Dot11CipherAlgoWpaUseGroup => CipherAlgorithm.WpaUseGroup,
            Dot11CipherAlgorithm.Dot11CipherAlgoWep => CipherAlgorithm.Wep,
            Dot11CipherAlgorithm.Dot11CipherAlgoIhvStart => CipherAlgorithm.IhvStart,
            Dot11CipherAlgorithm.Dot11CipherAlgoIhvEnd => CipherAlgorithm.IhvEnd,
            _ => CipherAlgorithm.Unknown
        };
    }
}