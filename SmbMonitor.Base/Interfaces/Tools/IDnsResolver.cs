using System.Net;

namespace SmbMonitor.Base.Interfaces.Tools;

public interface IDnsResolver
{
    IPAddress[] GetIpV4Addresses(string host);
}