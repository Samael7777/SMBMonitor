namespace SmbMonitorLib.Interfaces;

public interface IServiceController
{
    void StartServices();
    void StopServices();
    void SetLogger(ILogger logger);
}