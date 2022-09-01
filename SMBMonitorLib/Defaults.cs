namespace SmbMonitorLib;

public static class Defaults
{
    public static int SmbPort => 445;
    public static Timings Timings => new ()
    {
        Interval = 3000, 
        Timeout = 1000
    };
    public static bool SaveLogToFile = false;
    public static bool AutostartUnavailableSharesMonitor = true;
    public static string LogFileName = "Log.txt";
    public static string SettingsFileName = "settings.conf";
}
