using NetworkUtils;

namespace SmbMonitorLib.Services.Interfaces;

public interface IHostMonitoringService
{
    event Action<Host>? OnSmbAccessible;
    event Action<Host>? OnSmbUnaccessible;

    List<Host> Hosts { get; }
    
    bool IsStarted { get; }
    
    void AddHost(Host host);
    
    void RemoveHost(Host host);
    
    public void Start();
    
    public void Stop();
}