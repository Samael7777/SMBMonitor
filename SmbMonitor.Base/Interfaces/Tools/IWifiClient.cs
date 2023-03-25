using SmbMonitor.Base.DTO.Wifi;
using System.Net;
using System.Net.NetworkInformation;

namespace SmbMonitor.Base.Interfaces.Tools;

public interface IWifiClient
{
    IEnumerable<Guid> GetWlanInterfaces();
    WifiAdapter GetAdapterWithConnectedSsid(WifiSSID ssid);
    WifiSSID GetConnectedNetwork(Guid adapterId);
    NetworkInterface? GetInterfaceByGuid(Guid interfaceId);
    IPAddress GetConnectedAccessPointAddress(Guid adapterId);
}