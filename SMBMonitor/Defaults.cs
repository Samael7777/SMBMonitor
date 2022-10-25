using SmbMonitorLib;
using System.Diagnostics;
using System.IO;

namespace SMBMonitor;

public static class Defaults
{
    public static int SmbPort => 445;

    public static Timings Timings => new ()
    {
        Interval = 3000, 
        Timeout = 1000
    };
   
    public static bool SaveLogToFile => false;
   
    public static bool AutostartUnavailableSharesMonitor => true;
    
    public static string LogFileName => "Log.log";
    
    public static string SettingsFileName => "settings.conf";
    
    public static string MonitorsListFileName => "monitors.conf";

    public static string CurrentAppDir => Path.GetDirectoryName
        (Process.GetCurrentProcess().MainModule?.FileName) ?? "";

}
