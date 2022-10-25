using Base;
using NetworkUtils;

namespace SmbMonitorLib.Services.Interfaces;

public interface ISmbObserver
{
    public List<Host> SmbHosts { get; }
    public bool AddSmbServer(Host host, Credentials credentials);
    public bool RemoveSmbServer(Host host);
    public bool DisconnectSmbServer(Host host);
}