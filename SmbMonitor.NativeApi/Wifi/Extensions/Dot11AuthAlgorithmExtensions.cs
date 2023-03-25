using SmbMonitor.Base.DTO.Wifi;
using SmbMonitor.NativeApi.Wifi.Win32.DTO;

namespace SmbMonitor.NativeApi.Wifi.Extensions;

internal static class Dot11AuthAlgorithmExtensions
{
    public static AuthenticationAlgorithm Convert(this Dot11AuthAlgorithm source)
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
