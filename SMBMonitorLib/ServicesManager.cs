using Base;
using SmbMonitorLib.Services.Internal;

namespace SmbMonitorLib;

public static class ServicesManager
{
    static ServicesManager()
    { 
        WifiMonitoringService.Initialize();
        SmbMonitoringService.Initialize(HostMonitoringService.Instance, WindowsSharesMonitoringService.Instance);
        WifiMonitoringService.Instance.OnAccessPointConnected +=  SmbMonitoringService.Instance.AddSmbServer;
        WifiMonitoringService.Instance.OnAccessPointDisconnected +=
            (host) => SmbMonitoringService.Instance.RemoveSmbServer(host, true);
    }

    public static void SetLogger(ILogger logger)
    {
        SmbMonitoringService.Instance.Logger = logger;
        WifiMonitoringService.Instance.Logger = logger;
        SmbManager.Logger = logger;
        HostMonitoringService.Instance.Logger = logger;
        WindowsSharesMonitoringService.Instance.Logger = logger;
    }

    public static void StartServices()
    {
        SmbMonitoringService.Instance.Start();
        WifiMonitoringService.Instance.Start();
    }

    public static void StopServices()
    {
        WifiMonitoringService.Instance.Stop();
        SmbMonitoringService.Instance.Stop();
    }
}