// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using SmbMonitor.Base.DTO;
using SmbMonitor.Base.DTO.Nodes;
using SmbMonitor.Base.Interfaces;
using SmbMonitor.Base.Interfaces.Tools;

namespace SmbMonitor.Domain.Services;


public class SmbMonitoringService : ISmbMonitoringService
{
    private readonly Settings _settings;
    private readonly INodesStorageManager _nodesStorageManager;
    private readonly INodesSmbManager _nodesSmbManager;
    private readonly ISmbManager _smbManager;
    
    public SmbMonitoringService(
        INodesStorageManager nodesStorageManager, 
        INodesSmbManager nodesSmbManager,
        ISmbManager smbManager,
        Settings settings)
    {
        _nodesStorageManager = nodesStorageManager; 
        _nodesSmbManager = nodesSmbManager;
        _smbManager = smbManager;
        _settings = settings;
        
        InitConnectedShares();
    }

    public void OnNodeStatusChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not HostNode node) return;
        if (e.PropertyName != nameof(HostNode.SmbServerStatus)) return;

        switch (node.SmbServerStatus)
        {
            case SmbServerStatus.Connected:
                OnSmbServerConnected(node);
                break;
            case SmbServerStatus.Disconnected:
                _nodesSmbManager.DisconnectNodeShares(node);
                break;
        }
    }

    public void OnNodeListChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null) OnNodesAdded(e.NewItems.OfType<HostNode>());
        if (e.OldItems is not null) OnNodesRemoved(e.OldItems.OfType<HostNode>());
    }

    private void InitConnectedShares()
    {
        var connectedShares = _smbManager.GetConnectedShares();

        foreach (var share in connectedShares)
        {
            var host = new IPHostEntry { HostName = share.RemoteName.DnsSafeHost };

            var node = _nodesStorageManager.GetUnmanagedNode(host);
            if (node == null)
                _nodesStorageManager.AddUnmanagedNode(host, out node);
            
            _nodesSmbManager.AddOrUpdateNodeConnectedShare(node, share);
        }
    }

    private void OnSmbServerConnected(HostNode node)
    {
        _nodesSmbManager.UpdateNodeShares(node);

        if (!node.IsManaged) return;

        _nodesStorageManager.RemoveSameUnmanagedNodes(node);
        _nodesSmbManager.ConnectNodeShares(node);
    }

    private void OnNodesAdded(IEnumerable<HostNode> nodes)
    {
        Parallel.ForEach(nodes, node =>
        {
            _nodesSmbManager.GetNodeConnectedSharesFromOs(node);
            _nodesSmbManager.UpdateSmbStatus(node);

        });
    }

    private void OnNodesRemoved(IEnumerable<HostNode> nodes)
    {
        if (!_settings.DisconnectSharesAfterMonitorRemoved) return;
        
        Parallel.ForEach(nodes, node =>
        {
            if (node.IsManaged)
                _nodesSmbManager.DisconnectNodeShares(node);
        });
    }
}