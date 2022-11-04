using SmbMonitorLib.Services.Internal;

namespace SmbMonitorLib;

public static class ServiceController
{
    public static void StartServices()
    {
        SmbMonitoringService.Instance.Start();
        WifiMonitoringService.Instance.Start();
    }

    public static void StopServices()
    {
        SmbMonitoringService.Instance.Stop();
        WifiMonitoringService.Instance.Stop();
    }

    public static void BuildDependencies()
    {
        SmbManager.Initialize();
        WindowsSharesMonitoringService.Initialize();
        HostMonitoringService.Initialize();
        WifiMonitoringService.Initialize();
        SmbMonitoringService.Initialize(HostMonitoringService.Instance,
            WindowsSharesMonitoringService.Instance, SmbManager.Instance);

        WifiMonitoringService.Instance.OnAccessPointConnected += SmbMonitoringService.Instance.AddSmbServer;
        WifiMonitoringService.Instance.OnAccessPointDisconnected += SmbMonitoringService.Instance.RemoveSmbServerAndDisconnectShares;
    }

    public static void SetServicesLogger(ILogger logger)
    {
        SmbMonitoringService.Instance.Logger = logger;
        WifiMonitoringService.Instance.Logger = logger;
        SmbManager.Instance.Logger = logger;
        HostMonitoringService.Instance.Logger = logger;
        WindowsSharesMonitoringService.Instance.Logger = logger;
    }
}
