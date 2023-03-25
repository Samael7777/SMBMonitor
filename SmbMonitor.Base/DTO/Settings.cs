namespace SmbMonitor.Base.DTO;

public class Settings
{
    public int PollingIntervalMs { get; set; }
    public int ScanTimeoutMs { get; set; }
    public int SmbPort { get; set; }
    public int TriesToUnaccessible { get; set; }
    public int DnsCacheUpdateIntervalMs { get; set; }
    public bool MonitorUnmanagedShares { get; set; }
    public bool DisconnectSharesAfterMonitorRemoved { get; set; }
    public bool AutoConnectDisconnectedServerShares { get; set; }

}
public static class SettingsExtensions
{
    public static void Default(this Settings settings)
    {
        settings.PollingIntervalMs = 3000;
        settings.ScanTimeoutMs = 1000;
        settings.SmbPort = 445;
        settings.TriesToUnaccessible = 3;
        settings.AutoConnectDisconnectedServerShares = false;
        settings.MonitorUnmanagedShares = false;
        settings.DisconnectSharesAfterMonitorRemoved = false;
        settings.DnsCacheUpdateIntervalMs = 60000;
    }
}