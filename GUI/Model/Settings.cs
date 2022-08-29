using System.ComponentModel;
using System.Runtime.CompilerServices;
using SMBMonitor.Annotations;
using SmbMonitorLib;

namespace SMBMonitor.Model;

public class Settings
{
    public Timings Timings { get; set; }
    public int SmbPort { get; set; }
    public Settings()
    {
        Timings = Defaults.Timings;
        SmbPort = Defaults.SmbPort;
    }

}
