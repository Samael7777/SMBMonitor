using SmbMonitor.Base.DTO.Nodes;
using SmbMonitor.Base.DTO.Smb;

namespace SmbMonitor.Base.Interfaces.Tools;

public interface INodesSmbManager
{
    void ConnectNodeShares(HostNode node);
    void DisconnectNodeShares(HostNode node);
    void GetNodeAvailableSharesFromServer(HostNode node);
    void GetNodeConnectedSharesFromOs(HostNode node);
    void UpdateNodeShares(HostNode node);
    IEnumerable<SmbResourceInfo> GetNodeDisconnectedShares(HostNode node);
    void UpdateSmbStatus(HostNode node);
    void AddOrUpdateNodeConnectedShare(HostNode node, SmbResourceInfo share);
    void RemoveNodeConnectedShare(HostNode node, string driveLetter);
}