using NativeApi.Wifi.Base;
using NativeApi.Wifi.Win32;
using NativeApi.Wifi.Win32.DTO;
using WifiAPI.Win32.DTO;
using static NativeApi.Wifi.Win32.WifiAPI;

namespace NativeApi.Wifi;

public static class WifiAdapters
{
    public static List<WifiAdapter> GetAdaptersList()
    {
        using var handle = OpenHandle();
        var adapters = GetWlanInterfaces(handle);
        return adapters.Select(item => new WifiAdapter(item)).ToList();
    }

    public static WifiAdapter? GetAdapter(Guid id)
    {
        var adapters = GetAdaptersList();
        return adapters.FirstOrDefault(a => a.Id.Equals(id));
    }

    public static WifiAdapter? GetAdapterByConnectedNetwork(byte[] desiredSsid)
    {
        using var handle = OpenHandle();
        var connectedAdapters = GetConnectedAdapters(handle);

        return (from adapter in connectedAdapters
                let connectedNetwork = GetNetworkConnectedToAdapter
                    (handle, adapter.InterfaceGuid)
                where IsDesiredNetwork(connectedNetwork, desiredSsid)
                select new WifiAdapter(adapter))
            .FirstOrDefault();
    }

    public static WifiSSID GetConnectedNetwork(Guid adapterId)
    {
        using var handle = OpenHandle();
        var connectedNetwork = GetNetworkConnectedToAdapter(handle, adapterId);

        if (connectedNetwork is { } cn)
            return new WifiSSID(cn.dot11Ssid.ToBytes());

        return WifiSSID.Empty;
    }

    private static IEnumerable<WlanInterfaceInfo> GetConnectedAdapters(SafeWifiHandle handle)
    {
        return GetWlanInterfaces(handle)
            .Where(a => a.isState == WlanInterfaceState.Connected);
    }

    private static WlanAvailableNetwork? GetNetworkConnectedToAdapter
        (SafeWifiHandle handle, Guid adapterId)
    {
        var connectedNetwork = GetAvailableNetworks(handle, adapterId)
            .FirstOrDefault(n => new ConnectionFlags(n.Flags).Connected);

        return connectedNetwork;
    }

    private static bool IsDesiredNetwork(WlanAvailableNetwork? network, byte[] desiredSsid)
    {
        return network is { } currentNetwork
               && currentNetwork.dot11Ssid.ToBytes().SequenceEqual(desiredSsid);
    }
}