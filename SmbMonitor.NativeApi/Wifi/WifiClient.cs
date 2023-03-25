using SmbMonitor.Base.DTO.Wifi;
using SmbMonitor.Base.Interfaces.Tools;
using SmbMonitor.NativeApi.Wifi.Extensions;
using SmbMonitor.NativeApi.Wifi.Win32;
using SmbMonitor.NativeApi.Wifi.Win32.DTO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using static SmbMonitor.NativeApi.Wifi.Win32.WifiAPI;

namespace SmbMonitor.NativeApi.Wifi;

public class WifiClient : IWifiClient, IDisposable
{
    private readonly SafeWifiHandle _handle;

    public WifiClient()
    {
        _handle = OpenHandle();
    }
    public IEnumerable<Guid> GetWlanInterfaces()
    {
        var interfaceInfoList = WifiAPI.GetWlanInterfaces(_handle);
        return interfaceInfoList.Select(item => item.InterfaceGuid);
    }

    public WifiAdapter GetAdapterWithConnectedSsid(WifiSSID ssid)
    {
        var connectedAdapters = GetConnectedAdapters();
        return (from adapter in connectedAdapters
                let connectedNetwork = 
                    GetNetworkConnectedToAdapter (adapter.InterfaceGuid)
                where IsDesiredNetwork(connectedNetwork, ssid)
                select adapter.ToWifiAdapter())
            .FirstOrDefault();
    }

    public WifiSSID GetConnectedNetwork(Guid adapterId)
    {
        var connectedNetwork = GetNetworkConnectedToAdapter(adapterId);

        if (connectedNetwork is { dot11Ssid.SsidLength: > 0 } cn)
            return new WifiSSID(cn.dot11Ssid.ToBytes());

        return WifiSSID.Empty;
    }

    public NetworkInterface? GetInterfaceByGuid(Guid interfaceId)
    {
        if (interfaceId.Equals(Guid.Empty)) return null;

        return NetworkInterface.GetAllNetworkInterfaces()
            .FirstOrDefault(i => Guid.Parse(i.Id) == interfaceId);
    }

    public IPAddress GetConnectedAccessPointAddress(Guid adapterId)
    {
        var adapter = GetInterfaceByGuid(adapterId);
        if (adapter == null) return IPAddress.None;

        return adapter.GetIPProperties().GatewayAddresses
                   .FirstOrDefault(u =>
                       u.Address.AddressFamily == AddressFamily.InterNetwork)
                   ?.Address
               ?? IPAddress.None;
    }

    private IEnumerable<WlanInterfaceInfo> GetConnectedAdapters()
    {
        return WifiAPI.GetWlanInterfaces(_handle)
            .Where(a => a.isState == WlanInterfaceState.Connected);
    }

    private  WlanAvailableNetwork? GetNetworkConnectedToAdapter(Guid adapterId)
    {
        var connectedNetwork = GetAvailableNetworks(_handle, adapterId)
            .FirstOrDefault(n => ConnectionFlags.FromBitMask(n.Flags).Connected);

        return connectedNetwork;
    }

    private static bool IsDesiredNetwork(WlanAvailableNetwork? network, WifiSSID desiredSsid)
    {
        return network?.dot11Ssid.ToBytes().SequenceEqual(desiredSsid.SSIDBytes) ?? false;
    }

    #region Dispose

    private bool _disposed;

    ~WifiClient()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            //dispose managed state (managed objects)
            _handle.Dispose();
        } 
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null
        
        _disposed = true;
    }

    #endregion
}