using SmbMonitorLib;

namespace SMBMonitor.Model;

public class SettingsContainer
{
    public Timings Timings { get; set; } = new();
    public int SmbPort { get; set; }
    public bool AutostartUnavailableSharesMonitor { get; set; }
    public bool SaveLogToFile { get; set; }
    public string LogFilename { get; set; } = "";
}
