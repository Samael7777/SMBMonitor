using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using NativeApi.Smb.Base;
using SmbMonitorLib.Exceptions;
using SmbMonitorLib.Interfaces;
using SmbMonitorLib.Services.Base;

namespace SmbMonitorLib.Services;

//Todo Добавить настройку автоматического подключения отключенных дисков

internal class SmbMonitoringService : ControlledService<SmbMonitoringService>, ISmbMonitoringService
{
    private readonly ISmbService _smbService;
    private readonly IStorageService _storageService;

    public SmbMonitoringService(IStorageService storageService,
        ISmbService smbService, ISettings settings)
    {
        _smbService = smbService;
        _storageService = storageService;
        Logger = settings.Logger;
    }

    public void UpdateNodeStatus(MonitorNode node)
    {
        if (!_storageService.MonitorNodes.ContainsKey(node))
            throw new ServiceException("Node not exists in storage");

        var info = _storageService.MonitorNodes[node];
        var availableShares = _smbService.GetServerShares(node.Host, node.Credentials);
        if (availableShares.Any())
            UpdateNodeStatus(info, availableShares);
        else
            info.SmbStatus = SmbStatus.NoSharesAvailable;
    }

    protected override void OnStart()
    {
        _storageService.StorageItemChanged += OnStorageItemChanged;
        _storageService.StorageCollectionChanged += OnStorageChanged;
    }

    protected override void OnStop()
    {
        _storageService.StorageItemChanged -= OnStorageItemChanged;
        _storageService.StorageCollectionChanged -= OnStorageChanged;
    }

    private void OnStorageChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ProcessNewItems(e.NewItems);
        ProcessDeletedItems(e.OldItems);
    }
    
    private void OnStorageItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not MonitorNodeInfo info) return;
        if (e.PropertyName != nameof(MonitorNodeInfo.SmbServerStatus)) return;
        
        var node = info.LinkedNode;

        switch (info.SmbServerStatus)
        {
            case SmbServerStatus.Connected:
            {
                if (node.Type != NodeType.ExternalHost)
                    ConnectNodeSharesUpdateStatus(node);
                break;
            }
            case SmbServerStatus.Disconnected:
                DisconnectNodeSharesUpdateStatus(node);
                break;
            case SmbServerStatus.Unknown:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private void ProcessNewItems(IEnumerable? items)
    {
        if (items is null) return;
        var list = items.ToList<MonitorNode>()
            .Where(node => !node.Host.IPAddress.Equals(IPAddress.None));

        Parallel.ForEach(list, ConnectNodeSharesUpdateStatus);
    }

    //Todo Добавить настройку удаления шар при удалении из списка мониторинга
    private void ProcessDeletedItems(IEnumerable? items)
    {
        if (items is null) return;

        var list = items.ToList<MonitorNode>()
            .Where(node => node.Type is NodeType.AP or NodeType.Host);

        Parallel.ForEach(list, DisconnectNodeSharesUpdateStatus);
    }
    private void ConnectNodeSharesUpdateStatus(MonitorNode node)
    {
        var info = _storageService.MonitorNodes[node];
        var availableShares = _smbService.GetServerShares(node.Host, node.Credentials);
        info.SmbStatus = SmbStatus.Updating;
        UpdateNodeStatus(info, availableShares);
        
        if (info.SmbStatus != SmbStatus.Disconnected && info.SmbStatus != SmbStatus.PartiallyConnected)
            return;

        info.SmbStatus = SmbStatus.Connecting;

        var connectedShares = _smbService.GetServerConnectedShares(node.Host);
        var toConnect = availableShares.Except(connectedShares);
        ConnectNodeShares(toConnect, node.Credentials);
        
        UpdateNodeStatus(info, availableShares);
    }

    private void DisconnectNodeSharesUpdateStatus(MonitorNode node)
    {
        var info = _storageService.MonitorNodes[node];

        info.SmbStatus = SmbStatus.Updating;

        var connectedShares = _smbService.GetServerConnectedShares(node.Host);
        if (!connectedShares.Any())
        {
            info.SmbStatus = SmbStatus.Disconnected;
            return;
        }

        info.SmbStatus = SmbStatus.Disconnecting;
        DisconnectNodeShares(connectedShares);

        connectedShares = _smbService.GetServerConnectedShares(node.Host);
        info.SmbStatus = connectedShares.Any() 
            ? SmbStatus.PartiallyConnected 
            : SmbStatus.Disconnected;
    }
    
    private void UpdateNodeStatus(MonitorNodeInfo info, List<SmbResourceInfo> availableShares)
    {
        if (!availableShares.Any())
            info.SmbStatus = SmbStatus.NoSharesAvailable;

        var connectedShares = _smbService.GetServerConnectedShares(info.LinkedNode.Host);
        var disconnectedShares = availableShares.Except(connectedShares).ToList();

        switch (connectedShares.Count)
        {
            case > 0 when !disconnectedShares.Any():
                info.SmbStatus = SmbStatus.Connected;
                return;
            case > 0 when disconnectedShares.Any():
                info.SmbStatus = SmbStatus.PartiallyConnected;
                break;
            case 0 :
                info.SmbStatus = SmbStatus.Disconnected;
                break;
        }
    }

    private void ConnectNodeShares(IEnumerable<SmbResourceInfo> shares, Credentials credentials)
    {
        Parallel.ForEach(shares, share =>
        {
            _smbService.ConnectShareWithLettersReservation(share, credentials);
        });
    }
    
    private void DisconnectNodeShares(List<SmbResourceInfo> shares)
    {
        Parallel.ForEach(shares, (share) =>
        {
            _smbService.DisconnectShareAndFreeLetter(share);
        });
    }
}