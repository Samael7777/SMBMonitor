using WifiAPI.Base;
using WifiAPI.Win32;
using static WifiAPI.Win32.WifiAPI;

namespace WifiAPI;

public class WifiScanner
{
    public WifiScanner(Guid adapterId)
    {
        AdapterId = adapterId;
    }

    public Guid AdapterId { get; }

    public List<WifiNetworkInfo> GetAvailableNetworks()
    {
        using var handle = OpenHandle();

        var netsArray = Win32.WifiAPI.GetAvailableNetworks(handle, AdapterId);
        var result = ConvertNetworksArrayToList(netsArray);

        return result;
    }

    public WifiNetworkInfo? ConnectedNetwork()
    {
        var result = GetAvailableNetworks()
            .FirstOrDefault(n => n.Flags.Connected);

        return result;
    }

    public void Scan()
    {
        using var handle = OpenHandle();
        Win32.WifiAPI.Scan(handle, AdapterId);
    }

    private List<WifiNetworkInfo> ConvertNetworksArrayToList(WlanAvailableNetwork[] networks)
    {
        return networks.Select(n => new WifiNetworkInfo(n, AdapterId)).ToList();
    }
}