using SmbMonitorLib.Interfaces;

namespace SmbMonitorLib.Services;

internal class Settings : ISettings
{
    public int PollingIntervalMs { get; set; } = 3000;
    public int ScanTimeoutMs { get; set; } = 1000;
    public int Smb2Port { get; set; } = 445;
    public int Smb1Port { get; set; } = 139;
    public int TriesToUnaccessible { get; set; } = 3;
    public ILogger? Logger { get; set; }
    public bool DisconnectUnavailableExternalShares { get; set; }
}