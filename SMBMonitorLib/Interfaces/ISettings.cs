namespace SmbMonitorLib.Interfaces;

public interface ISettings
{
    int PollingIntervalMs { get; set; }
    int ScanTimeoutMs { get; set; }
    int Smb2Port { get; set; }
    int Smb1Port { get; set; }
    int TriesToUnaccessible { get; set; }
    bool DisconnectUnavailableExternalShares { get; set; }
    ILogger? Logger { get; set; }
}