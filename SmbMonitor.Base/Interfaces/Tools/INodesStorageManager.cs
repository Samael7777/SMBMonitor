using System.Net;
using SmbMonitor.Base.DTO.Nodes;

namespace SmbMonitor.Base.Interfaces.Tools;

public interface INodesStorageManager
{
    void RemoveSameUnmanagedNodes(HostNode node);
    void AddUnmanagedNode(IPHostEntry host, out HostNode newNode);
    HostNode? GetUnmanagedNode(IPHostEntry host);
    HostNode? GetNodeWithConnectedShare(string driveLetter);
    HostNode? GetNodeByHost(IPHostEntry host);
}