using System.Collections;
using System.Collections.Specialized;
using System.Net;
using Base;
using NetworkUtils;
using SmbMonitorLib.Exceptions;
using SmbMonitorLib.Services.Base;
using SmbMonitorLib.Services.DTO;
using SmbMonitorLib.Services.Interfaces;
using SmbMonitorLib.Services.Internal;
using SMBMonitorLib.Services.Base;
using Swordfish.NET.Collections;

namespace SmbMonitorLib;

public class SmbMonitoringService : ControlledService<SmbMonitoringService>
{
    private static SmbMonitoringService? instance;
    private readonly IHostMonitoringService _hostObserver;
    private readonly IWindowsSharesMonitoringService _sharesObserver;

    private SmbMonitoringService(IHostMonitoringService hostObserver, IWindowsSharesMonitoringService sharesObserver)
    {
        _hostObserver = hostObserver;
        _sharesObserver = sharesObserver;
        SmbServers.CollectionChanged += OnSmbServersCollectionChanged;
    }

    public static SmbMonitoringService Instance
    {
        get
        {
            if (instance == null)
                throw new InitializeException(nameof(Initialize));

            return instance;
        }
    }
    
    public static void Initialize(IHostMonitoringService _hostObserver, IWindowsSharesMonitoringService sharesObserver)
    {
        if (instance != null) throw new AlreadyInitializedException();
        instance = new SmbMonitoringService(_hostObserver, sharesObserver);
    }
    
    public ConcurrentObservableDictionary<Host, SmbMonitoringData> SmbServers { get; } = new();
    
    public void AddSmbServer(Host host, Credentials credentials)
    {
        if (host.IPAddress.Equals(IPAddress.None)) return;
        if (SmbServers.ContainsKey(host)) throw new ItemExistsException();

        var info = new SmbMonitoringData
        {
            Credentials = credentials
        };
        SmbServers.Add(host, info);

        if (!_hostObserver.Hosts.Contains(host)) _hostObserver.AddHost(host);
        var address = host.IPAddress;

        LogWriteLine($"Добавлен хост {address}.");
    }

    public void RemoveSmbServer(Host host, bool forceDisconnectShares = false)
    {
        if (!SmbServers.ContainsKey(host)) throw new ItemNotExistsException();

        if (forceDisconnectShares) DisconnectAllObservableServerShares(host);
        if (_hostObserver.Hosts.Contains(host)) _hostObserver.RemoveHost(host);
        
        SmbServers.Remove(host);

        LogWriteLine($"Удален хост {host.IPAddress}.");
    }

    protected override void OnStart()
    {
        _hostObserver.OnSmbAccessible += OnSmbServerAccessible;
        _hostObserver.OnSmbUnaccessible += OnSmbServerUnaccessible;
        _sharesObserver.OnConnectedSharesListChanged += OnConnectedSharesListChanged;

        if (!_hostObserver.IsStarted) _hostObserver.Start();
        if (!_sharesObserver.IsStarted) _sharesObserver.Start();

        AddConnectedServersToObserver();
    }

    protected override void OnStop()
    {
        if (_hostObserver.IsStarted) _hostObserver.Stop();
        if (_sharesObserver.IsStarted) _sharesObserver.Stop();

        _hostObserver.OnSmbAccessible -= OnSmbServerAccessible;
        _hostObserver.OnSmbUnaccessible -= OnSmbServerUnaccessible;
        _sharesObserver.OnConnectedSharesListChanged -= OnConnectedSharesListChanged;
    }

    private void OnSmbServersCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null) ProcessAddedItems(e.NewItems);
        if (e.OldItems != null) ProcessRemovedItems(e.OldItems);
    }

    private void OnSmbServerAccessible(Host host)
    {
        if (!SmbServers.ContainsKey(host)) return;

        SmbServers[host].IsHostAvailable = true;
        ConnectAllServerShares(host);
    }

    private void OnSmbServerUnaccessible(Host host)
    {
        if (SmbServers.ContainsKey(host))
        {
            SmbServers[host].IsHostAvailable = false;
            DisconnectAllObservableServerShares(host);
        }
        else
            DisconnectAllServerShares(host);
    }

    private void OnConnectedSharesListChanged()
    {
        var connectedServers = SmbManager.GetConnectedServers();
        foreach (var server in connectedServers)
            if (!_hostObserver.Hosts.Contains(server))
                _hostObserver.Hosts.Add(server);

        foreach (var host in _hostObserver.Hosts.Where(IsHostOrphaned)) _hostObserver.RemoveHost(host);
    }

    private void ProcessAddedItems(IEnumerable items)
    {
        foreach (var item in items)
        {
            if (item is not KeyValuePair<Host, SmbMonitoringData> monitoringPair) continue;

            var server = monitoringPair.Key;

            LogWriteLine($"Добавлен SMB-сервер {server.IPAddress}");

            if (!_hostObserver.Hosts.Contains(server)) _hostObserver.AddHost(server);
        }
    }

    private void ProcessRemovedItems(IEnumerable items)
    {
        foreach (var item in items)
        {
            if (item is not KeyValuePair<Host, SmbMonitoringData> monitoringPair) continue;

            var server = monitoringPair.Key;

            LogWriteLine($"Удален SMB-сервер {server.IPAddress}");

            if (_hostObserver.Hosts.Contains(server)) _hostObserver.RemoveHost(server);
        }
    }

    private void AddConnectedServersToObserver()
    {
        var servers = SmbManager.GetConnectedServers();
        foreach (var server in servers)
            if (!_hostObserver.Hosts.Contains(server))
                _hostObserver.AddHost(server);
    }
    
    private void ConnectAllServerShares(Host host)
    {
        if (SmbServers[host].Status is not (SmbStatus.Disconnected or SmbStatus.Unknown)) return;

        SetServerStatus(host, SmbStatus.Connecting);

        var credentials = SmbServers[host].Credentials;
        var disconnectedShares = SmbManager.GetServerDisconnectedShares(host, credentials);
        
        SmbManager.ConnectSharesWithLettersReservation(disconnectedShares, credentials);

        SetServerStatus(host, SmbStatus.Connected);
    }

    private void DisconnectAllObservableServerShares(Host host)
    {
        if (SmbServers[host].Status is not (SmbStatus.Connected or SmbStatus.Unknown)) return;

        SetServerStatus(host, SmbStatus.Disconnecting);
        DisconnectAllServerShares(host);
        SetServerStatus(host, SmbStatus.Disconnected);
    }

    private void DisconnectAllServerShares(Host host)
    {
        SmbManager.DisconnectAllShares(host);
        
        LogWriteLine(!SmbManager.GetServerConnectedShares(host).Any()
            ? $"Все подключенные ресурсы SMB-сервера {host.IPAddress} отключены"
            : $"Имеются ошибки отключения ресурсов SMB-сервера {host.IPAddress}");
    }

    private void SetServerStatus(Host host, SmbStatus status)
    {
        SmbServers[host].ConnectedShares = SmbManager.GetServerConnectedShares(host).Count();
        SmbServers[host].Status = status;
    }

    private bool IsHostOrphaned(Host host)
    {
        var connectedServers = SmbManager.GetConnectedServers();
        return !SmbServers.ContainsKey(host) && !connectedServers.Contains(host);
    }

}