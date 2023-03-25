using System.Net;

namespace SmbMonitor.Base.Interfaces.Tools;

public interface IPortScanner
{
    bool IsPortOpen(IPAddress address, int port, int timeout);
}
