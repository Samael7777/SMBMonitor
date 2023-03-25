using SmbMonitor.Base.DTO.Wifi;
using SmbMonitor.NativeApi.Wifi.Win32.DTO;

namespace SmbMonitor.NativeApi.Wifi.Extensions;

internal static class Dot11BssTypeExtensions
{
    public static BssType ToBssType(this Dot11BssType source)
    {
        return source switch
        {
            Dot11BssType.Infrastructure => BssType.Infrastructure,
            Dot11BssType.Independent => BssType.Independent,
            _ => BssType.None
        };
    }
}
