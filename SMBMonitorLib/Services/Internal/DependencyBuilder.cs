namespace SmbMonitorLib.Services.Internal;

internal static class DependencyBuilder
{
    static DependencyBuilder()
    {
        WifiMonitoringService.Instance.OnAccessPointConnected +=  SmbMonitoringService.Instance.AddSmbServer;
        WifiMonitoringService.Instance.OnAccessPointDisconnected += SmbMonitoringService.Instance.RemoveSmbServerAndDisconnectShares;
    }
}
