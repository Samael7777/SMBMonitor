using System.Net;
using SmbMonitor.Base.DTO.Nodes;

namespace SmbMonitor.Base.Extensions;

public static class NodeExtentions
{
    public static void SetSmbServerConnectedResetTries(this HostNode node)
    {
        node.SmbCheckTries = 0;
        node.SmbServerStatus = SmbServerStatus.Connected;
    }

    public static void SetSmbServerDisconnectedReachingTriesLimit(this HostNode node, int triesLimit)
    {
        if (node.SmbCheckTries < triesLimit)
            node.SmbCheckTries++;
        else
            node.SmbServerStatus = SmbServerStatus.Disconnected;
    }

    public static void SetWifiApConnected(this WifiNode node, Guid adapterId, IPAddress apIpAddress)
    {
        node.AdapterId = adapterId;
        node.Host.HostName = apIpAddress.ToString();
        node.Host.AddressList = new[] { apIpAddress };
        node.IsAPConnected = true;
    }

    public static void SetWifiApSmbServerDisconnected(this WifiNode node)
    {
        node.IsAPConnected = false;
        node.SmbServerStatus = SmbServerStatus.Disconnected;
    }

    public static void SetScanning(this HostNode node)
    {
        node.IsScanning = true;
    }

    public static void SetNotScanning(this HostNode node)
    {
        node.IsScanning = false;
    }
}
