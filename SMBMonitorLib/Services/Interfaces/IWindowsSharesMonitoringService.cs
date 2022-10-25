namespace SmbMonitorLib.Services.Interfaces;

public interface IWindowsSharesMonitoringService
{
    event Action? OnConnectedSharesListChanged;

    bool IsStarted { get; }
    
    void Start();
    
    void Stop();
}