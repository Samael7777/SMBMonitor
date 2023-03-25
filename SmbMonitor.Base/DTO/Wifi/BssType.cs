namespace SmbMonitor.Base.DTO.Wifi;

/// <summary>
///     BSS network type
/// </summary>
/// <remarks>
///     Partly equivalent to DOT11_BSS_TYPE:
///     https://docs.microsoft.com/en-us/windows/win32/nativewifi/dot11-bss-type
///     Also equivalent to connectionType element in profile XML:
///     https://docs.microsoft.com/en-us/windows/win32/nativewifi/wlan-profileschema-connectiontype-wlanprofile-element
/// </remarks>
public enum BssType
{
    /// <summary>
    ///     None (invalid value)
    /// </summary>
    None = 0,

    /// <summary>
    ///     Infrastructure BSS network
    /// </summary>
    Infrastructure,

    /// <summary>
    ///     Independent BSS (IBSS) network (Ad hoc network)
    /// </summary>
    Independent
}