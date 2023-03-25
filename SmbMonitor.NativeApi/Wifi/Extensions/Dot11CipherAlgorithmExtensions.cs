using SmbMonitor.Base.DTO.Wifi;
using SmbMonitor.NativeApi.Wifi.Win32.DTO;

namespace SmbMonitor.NativeApi.Wifi.Extensions;

internal static class Dot11CipherAlgorithmExtensions
{
    public static CipherAlgorithm ToCipherAlgorithm(this Dot11CipherAlgorithm source)
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