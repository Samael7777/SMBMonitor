using SmbMonitor.Base.Comparers;
using SmbMonitor.Base.DTO.Nodes;
using SmbMonitor.Base.DTO.Smb;
using SmbMonitor.Base.Interfaces.Tools;
using System.Collections.Immutable;
using SmbMonitor.Base.Extensions;

namespace SmbMonitor.Domain.Tools;

public class NodesSmbManager : INodesSmbManager
{
    private readonly ISmbManager _smbManager;

    public NodesSmbManager(ISmbManager smbManager)
    {
        _smbManager = smbManager;
    }

    public void ConnectNodeShares(HostNode node)
    {
        var disconnectedShares = GetNodeDisconnectedShares(node)
            .ToImmutableArray();

        if (!disconnectedShares.Any()) return;

        var prevStatus = node.SmbStatus;
        node.SmbStatus = SmbStatus.Connecting;
        
        if (!_smbManager.TryConnectSharesWithAutoLetterReservation(disconnectedShares, node.Credentials))
            node.SmbStatus = prevStatus;
    }

    public void DisconnectNodeShares(HostNode node)
    {
        var connectedShares = node.ConnectedShares.Values
            .ToImmutableArray();

        if (!connectedShares.Any()) return;

        var prevStatus = node.SmbStatus;
        node.SmbStatus = SmbStatus.Disconnecting;
        
        if (!_smbManager.TryDisconnectSharesFreeLetters(connectedShares))
            node.SmbStatus = prevStatus;
    }

    public void GetNodeAvailableSharesFromServer(HostNode node)
    {
        node.AvailableShares.Clear();
        if (node.SmbServerStatus != SmbServerStatus.Connected) return;

        _smbManager.GetServerShares(node.Host.HostName, node.Credentials)
            .ForEach(share => 
                node.AvailableShares.TryAdd(share.RemoteName.AbsolutePath, share));
    }

    public void GetNodeConnectedSharesFromOs(HostNode node)
    {
        node.ConnectedShares.Clear();

        var hostname = node.Host.HostName;
        if (string.IsNullOrWhiteSpace(hostname)) return;

        _smbManager.GetServerConnectedShares(hostname)
            .ForEach(share => AddOrUpdateNodeConnectedShare(node, share));
    }

    public void AddOrUpdateNodeConnectedShare(HostNode node, SmbResourceInfo share)
    {
        node.ConnectedShares.AddOrUpdate(share.MappedDisk,
            static (_, newShare) => newShare,
            static (_, _, newShare) => newShare,
            share);
    }

    public void RemoveNodeConnectedShare(HostNode node, string driveLetter)
    {
        if (!node.ConnectedShares.ContainsKey(driveLetter)) return;

        node.ConnectedShares.TryRemove(driveLetter, out _);
    }

    public IEnumerable<SmbResourceInfo> GetNodeDisconnectedShares(HostNode node)
    {
        return node.AvailableShares.Values.Except(node.ConnectedShares.Values, new HostSharesComparer());
    }

    public void UpdateSmbStatus(HostNode node)
    {
        var isConnectedShares = node.ConnectedShares.Any();
        var isAvailableShares = node.AvailableShares.Any();
        var isDisconnectedShares = GetNodeDisconnectedShares(node).Any();

        if (isAvailableShares)
        {
            if (isConnectedShares)
                node.SmbStatus = isDisconnectedShares ? SmbStatus.PartiallyConnected : SmbStatus.Connected;
            else
                node.SmbStatus = SmbStatus.Disconnected;
        }
        else
        {
            node.SmbStatus = isConnectedShares ? SmbStatus.PartiallyConnected : SmbStatus.NoSharesAvailable;
        }
    }

    public void UpdateNodeShares(HostNode node)
    {
        node.SmbStatus = SmbStatus.Updating;

        GetNodeAvailableSharesFromServer(node);
        GetNodeConnectedSharesFromOs(node);
        UpdateSmbStatus(node);
    }
}
