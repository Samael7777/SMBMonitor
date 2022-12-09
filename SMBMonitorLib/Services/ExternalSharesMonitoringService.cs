
using System.Net;
using NativeApi.MessageCapture;
using NativeApi.NetworkUtils;
using NativeApi.Smb.Base;
using SmbMonitorLib.Exceptions;
using SmbMonitorLib.Interfaces;
using SmbMonitorLib.Services.Base;

namespace SmbMonitorLib.Services;

internal class ExternalSharesMonitoringService : 
    ControlledService<ExternalSharesMonitoringService>, IExternalSharesMonitorService
{
    private readonly Dictionary<char, SmbResourceInfo> _lastConnectedShares = new();
    private DriveStateCapture? _driveStateCapture;
    private readonly ISmbService _smbService;
    private readonly IStorageService _storageService;
    private readonly ISmbMonitoringService _smbMonitoringService;
    private readonly ISettings _settings;

    public ExternalSharesMonitoringService(IStorageService storageService, ISmbService smbService,
        ISmbMonitoringService smbMonitoringService, ISettings settings)
    {
        _storageService = storageService;
        _smbService = smbService;
        _smbMonitoringService = smbMonitoringService;
        Logger = settings.Logger;
        _settings = settings;
    }

    protected override void OnStart()
    {
        if (_driveStateCapture is not null) return;
        lock (_lastConnectedShares)
        {
            SetLastConnectedShares();
        }
        AddExternalSharesOnInit();
        _driveStateCapture = new DriveStateCapture();
        _driveStateCapture.OnDriveStateChange += DriveStateChange;
    }

    protected override void OnStop()
    {
        if (_driveStateCapture is null) return;
        _driveStateCapture.OnDriveStateChange -= DriveStateChange;
        _driveStateCapture.Dispose();
        _driveStateCapture = null;
    }

    private void AddExternalSharesOnInit()
    {
        lock (_lastConnectedShares)
        {
            foreach (var share in _lastConnectedShares.Values)
            {
                Host server;
                var remoteName = share.RemoteName;
                server = share.RemoteName.ServerAddress.Equals(IPAddress.None) 
                    ? new Host(remoteName.ServerName) 
                    : new Host(remoteName.ServerAddress);

                if (_storageService.MonitorNodes.Keys.All(n => n.Host != server))
                    _storageService.AddExternalHost(server);
            }
        }
    }
    
    private void DriveStateChange(char driveLetter, DriveType driveType, DriveState state)
    {
        switch (state)
        {
            case DriveState.Arrival:
                ProcessConnection(driveLetter);
                break;
            case DriveState.RemoveComplete:
                ProcessRemove(driveLetter);
                break;
        }
    }
    
    private void ProcessConnection(char driveLetter)
    {
        lock (_lastConnectedShares)
        {
            SetLastConnectedShares();
            if (!_lastConnectedShares.ContainsKey(driveLetter)) return;

            var server = GetHostByDriveLetter(driveLetter);

            var node = GetNodeByHost(server);
            if (node is null)
            {
                if (_settings.DisconnectUnavailableExternalShares)
                    _storageService.AddExternalHost(server);
            }
            else if (node.Type is NodeType.AP or NodeType.Host)
            {
                _smbMonitoringService.UpdateNodeStatus(node);
            }
        }
    }

    private void ProcessRemove(char driveLetter)
    {
        lock (_lastConnectedShares)
        {
            if (!_lastConnectedShares.ContainsKey(driveLetter)) return;
            var server = GetHostByDriveLetter(driveLetter);
            SetLastConnectedShares();

            var node = GetNodeByHost(server);
            if (node is null) return;
            switch (node.Type)
            {
                case NodeType.AP or NodeType.Host:
                    _smbMonitoringService.UpdateNodeStatus(node);
                    break;
                case NodeType.ExternalHost:
                {
                    var leftConnected = _smbService.GetServerConnectedShares(server).Count;
                    if (leftConnected == 0)
                        _storageService.RemoveItem(node);
                    break;
                }
            }
        }
    }
    
    private void SetLastConnectedShares()
    {
        _lastConnectedShares.Clear();
        var connectedShares = _smbService.GetConnectedShares();
        foreach (var share in connectedShares)
        {
            var letter = char.ToUpper(share.LocalName[0]);
            if (!_lastConnectedShares.TryAdd(letter, share))
                throw new ServiceException("Last connected shares storage error.");
        }
    }

    private Host GetHostByDriveLetter(char driveLetter)
    {
        var share = _lastConnectedShares[driveLetter];
        var server = new Host(share.RemoteName.ServerAddress);
        return server;
    }

    private MonitorNode? GetNodeByHost(Host host)
    {
        var node = _storageService.MonitorNodes.Keys
            .FirstOrDefault(n => n.Host.Equals(host));
        return node;
    }
}