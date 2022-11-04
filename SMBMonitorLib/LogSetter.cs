using SmbMonitorLib.Services.Internal;

namespace SmbMonitorLib;

public static class LogSetter
{
    public static void SetLogger(ILogger logger)
    {
        SmbMonitoringService.Instance.Logger = logger;
        WifiMonitoringService.Instance.Logger = logger;
        SmbManager.Logger = logger;
        HostMonitoringService.Instance.Logger = logger;
        WindowsSharesMonitoringService.Instance.Logger = logger;
    }
}