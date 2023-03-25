using System.Net;
using SmbMonitor.Base.DTO;
using SmbMonitor.Base.Interfaces;
using SmbMonitor.Base.Interfaces.Tools;

namespace SmbMonitor.Domain.Services;

public class ConnectedDisksMonitoringService : IConnectedDisksMonitoringService
{
    private readonly Settings _settings;
    private readonly INodesStorageManager _nodesStorageManager;
    private readonly INodesSmbManager _nodesSmbManager;
    private readonly ISmbManager _smbManager;

    public ConnectedDisksMonitoringService(ISmbManager smbManager, 
        INodesStorageManager nodesStorageManager, INodesSmbManager nodesSmbManager,
        Settings settings)
    {
        _nodesStorageManager = nodesStorageManager; 
        _nodesSmbManager = nodesSmbManager;
        _smbManager = smbManager;
        _settings = settings;
    }

    public void OnDriveStateChange(string driveLetter, DriveType driveType, DriveState state)
    {
        switch (state)
        {
            case DriveState.Arrival when driveType == DriveType.Network:
                OnDiskAttached(driveLetter);
                break;
            case DriveState.RemoveComplete:
                OnDiskRemoved(driveLetter);
                break;
            case DriveState.Unknown:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void OnDiskAttached(string driveLetter)
    {
        var connectedShares = _smbManager.GetConnectedShares();
        var attachedShare = connectedShares
            .FirstOrDefault(s => s.MappedDisk == driveLetter);

        if (attachedShare.IsEmpty) return;

        var host = new IPHostEntry { HostName = attachedShare.RemoteName.DnsSafeHost };
        var node = _nodesStorageManager.GetNodeByHost(host);

        switch (node)
        {
            case null when !_settings.MonitorUnmanagedShares:
                return;
            case null:
                _nodesStorageManager.AddUnmanagedNode(host, out node);
                break;
        }
        _nodesSmbManager.AddOrUpdateNodeConnectedShare(node, attachedShare);
        _nodesSmbManager.UpdateSmbStatus(node);
    }

    private void OnDiskRemoved(string driveLetter)
    {
        var node = _nodesStorageManager.GetNodeWithConnectedShare(driveLetter);
        
        if (node is null) return;

        _nodesSmbManager.RemoveNodeConnectedShare(node, driveLetter);
        _nodesSmbManager.UpdateSmbStatus(node);
    }
}