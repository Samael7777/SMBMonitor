using System.Net;

namespace SmbMonitor.Base.Interfaces.Tools;

public interface IPortScannerAsync
{
    Task<bool> IsPortOpenAsync(IPAddress address, int port, int timeout);
}
