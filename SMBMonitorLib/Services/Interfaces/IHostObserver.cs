using NetworkUtils;

namespace SmbMonitorLib.Services.Interfaces;

public interface IHostObserver
{
    public List<Host> Hosts { get; }
    public void AddHost(Host host);
    public void RemoveHost(Host host);
}