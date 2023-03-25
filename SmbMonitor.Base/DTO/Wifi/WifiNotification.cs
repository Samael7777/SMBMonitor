// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global

namespace SmbMonitor.Base.DTO.Wifi;

/// <summary>
///     Enumerator for wlan notification type
/// </summary>
public enum WifiNotification : uint
{
    Start = 0,
    AutoconfEnabled,
    AutoconfDisabled,
    BackgroundScanEnabled,
    BackgroundScanDisabled,
    BssTypeChange,
    PowerSettingChange,
    ScanComplete,
    ScanFail,
    ConnectionStart,
    ConnectionComplete,
    ConnectionAttemptFail,
    FilterListChange,
    InterfaceArrival,
    InterfaceRemoval,
    ProfileChange,
    ProfileNameChange,
    ProfilesExhausted,
    NetworkNotAvailable,
    NetworkAvailable,
    Disconnecting,
    Disconnected,
    AdhocNetworkStateChange,
    ProfileUnblocked,
    ScreenPowerChange,
    ProfileBlocked,
    ScanListRefresh,
    End
}