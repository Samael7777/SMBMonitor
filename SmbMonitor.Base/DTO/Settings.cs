namespace SmbMonitor.Base.DTO;

public class Settings
{
    public int PollingIntervalMs;
    public int ScanTimeoutMs;
    public int SmbPort;
    public int TriesToUnaccessible;
    public int DnsCacheUpdateIntervalMs;
    public bool MonitorUnmanagedShares;
    public bool DisconnectSharesAfterMonitorRemoved;
    public bool AutoConnectDisconnectedServerShares;
}
public static class SettingsExtensions
{
    public static Settings SetDefault(this Settings settings)
    {
        settings.PollingIntervalMs = 3000;
        settings.ScanTimeoutMs = 1000;
        settings.SmbPort = 445;
        settings.TriesToUnaccessible = 3;
        settings.AutoConnectDisconnectedServerShares = false;
        settings.MonitorUnmanagedShares = false;
        settings.DisconnectSharesAfterMonitorRemoved = false;
        settings.DnsCacheUpdateIntervalMs = 60000;
        return settings;
    }
}