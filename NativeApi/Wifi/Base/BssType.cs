using WifiAPI.Win32.DTO;

namespace NativeApi.Wifi.Base;

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

internal static class BssTypeConverter
{
    public static BssType Convert(Dot11BssType source)
    {
        return source switch
        {
            Dot11BssType.Infrastructure => BssType.Infrastructure,
            Dot11BssType.Independent => BssType.Independent,
            _ => BssType.None
        };
    }

    public static Dot11BssType ConvertBack(BssType source)
    {
        return source switch
        {
            BssType.Infrastructure => Dot11BssType.Infrastructure,
            BssType.Independent => Dot11BssType.Independent,
            _ => throw new ArgumentException($"Invalid value in {nameof(source)}")
        };
    }
}