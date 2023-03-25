using SmbMonitor.Base.DTO.Nodes;
using SmbMonitor.Base.Extensions;
using SmbMonitor.Base.Interfaces;
using System.Net;
using SmbMonitor.Base.Interfaces.Tools;

namespace SmbMonitor.Domain.Tools;

public class NodesStorageManager : INodesStorageManager
{
    private readonly IEqualityComparer<IPHostEntry> _hostsComparer;
    private readonly IStorage _storage;

    public NodesStorageManager(IStorage storage, 
        IEqualityComparer<IPHostEntry> hostsComparer)
    {
        _storage = storage;
        _hostsComparer = hostsComparer;
    }

    public void RemoveSameUnmanagedNodes(HostNode node)
    {
        if (!node.IsManaged) return;
      
        var unmanagedNodes = _storage.MonitorNodes
            .Where(n => !n.IsManaged);

        var  nodesToRemove = unmanagedNodes
            .Where(n => _hostsComparer.Equals(n.Host, node.Host));

        nodesToRemove.ForEach(_storage.RemoveItem);
    }

    public void AddUnmanagedNode(IPHostEntry host, out HostNode newNode)
    {
        newNode = new HostNode(host, default, false);
        _storage.AddItem(newNode);
    }

    public HostNode? GetUnmanagedNode(IPHostEntry host)
    {

        return _storage.MonitorNodes.FirstOrDefault(node => 
                _hostsComparer.Equals(node.Host, host));
    }

    public HostNode? GetNodeWithConnectedShare(string driveLetter)
    {
        return _storage.MonitorNodes
            .FirstOrDefault(n => n.ConnectedShares.ContainsKey(driveLetter));
    }

    public HostNode? GetNodeByHost(IPHostEntry host)
    {
        return _storage.MonitorNodes.FirstOrDefault(n => 
            _hostsComparer.Equals(n.Host, host));
    }
}