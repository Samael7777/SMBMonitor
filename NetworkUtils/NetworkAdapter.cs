using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace NetworkUtils;

public class NetworkAdapter
{
    public NetworkAdapter(Guid id)
    {
        Id = id;
    }

    private NetworkInterface? Adapter => GetAdapter();

    public Guid Id { get; }

    public IPAddress InterfaceIp => GetInterfaceIP();

    public IPAddress SubnetMask => GetSubnetMask();

    public IPAddress GatewayIp => GetGatewayIP();

    public string Description => Adapter?.Description ?? string.Empty;

    public OperationalStatus Status => Adapter?.OperationalStatus ?? OperationalStatus.NotPresent;

    public NetworkInterfaceType InterfaceType => Adapter?.NetworkInterfaceType ?? NetworkInterfaceType.Unknown;

    private NetworkInterface? GetAdapter()
    {
        return NetworkInterface.GetAllNetworkInterfaces()
            .FirstOrDefault(i => Guid.Parse(i.Id) == Id);
    }

    private IPAddress GetInterfaceIP()
    {
        return Adapter?.GetIPProperties().UnicastAddresses
                   .FirstOrDefault(u =>
                       u.Address.AddressFamily == AddressFamily.InterNetwork)?.Address
               ?? IPAddress.None;
    }

    private IPAddress GetSubnetMask()
    {
        return Adapter?.GetIPProperties().UnicastAddresses
                   .FirstOrDefault(u =>
                       u.IPv4Mask.AddressFamily == AddressFamily.InterNetwork)?.IPv4Mask
               ?? IPAddress.None;
    }

    private IPAddress GetGatewayIP()
    {
        return Adapter?.GetIPProperties().GatewayAddresses
                   .FirstOrDefault(u =>
                       u.Address.AddressFamily == AddressFamily.InterNetwork)?.Address
               ?? IPAddress.None;
    }
}