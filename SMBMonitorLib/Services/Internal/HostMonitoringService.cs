using System.Collections.Concurrent;
using NetworkUtils;
using SmbMonitorLib.Exceptions;
using SmbMonitorLib.Services.Base;
using SmbMonitorLib.Services.DTO;
using SmbMonitorLib.Services.Interfaces;
using SMBMonitorLib.Services.Base;

namespace SmbMonitorLib.Services.Internal;

internal class HostMonitoringService : ControlledService<HostMonitoringService>, IHostMonitoringService
{
    private static HostMonitoringService? instance;
    private readonly ConcurrentDictionary<Host, HostMonitoringInfo> _hosts = new();
    private Timer? _pollingTimer;

    private HostMonitoringService()
    {
    }

    public static HostMonitoringService Instance
    {
        get
        {
            instance ??= new HostMonitoringService();
            return instance;
        }
    }
    
    public int PollingIntervalMs { get; set; } = 3000;

    public int ScanTimeoutMs { get; set; } = 1000;

    public int SmbPort { get; set; } = 445;

    public event Action<Host>? OnSmbAccessible;
    public event Action<Host>? OnSmbUnaccessible;

    public List<Host> Hosts => _hosts.Keys.ToList();

    public void AddHost(Host host)
    {
        if (_hosts.ContainsKey(host))
            throw new ItemExistsException();
        if (!_hosts.TryAdd(host, new HostMonitoringInfo()))
            throw new StorageException();

        LogWriteLine($"Добавлен хост {host.IPAddress}.");
    }

    public void RemoveHost(Host host)
    {
        if (!_hosts.ContainsKey(host))
            throw new ItemNotExistsException();
        if (!_hosts.Remove(host, out _))
            throw new StorageException();

        LogWriteLine($"Удален хост {host.IPAddress}.");
    }

    protected override void OnStart()
    {
        void TimerCallbackProc(object? args)
        {
            Parallel.ForEach(_hosts.Keys, ScanTask);
        }

        _pollingTimer = new Timer
        (
            TimerCallbackProc,
            null,
            0,
            PollingIntervalMs
        );
    }

    protected override void OnStop()
    {
        _pollingTimer?.Dispose();
        _pollingTimer = null;
    }

    private void ScanTask(Host host)
    {
        if (_hosts[host].IsScanning) return;
        if (IsPortOpen(host))
            OnSmbPortOpened(host);
        else
            OnSmbPortClosed(host);
    }

    private bool IsPortOpen(Host host)
    {
        _hosts[host].IsScanning = true;
        var isPortOpen = new PortChecker(host.IPAddress, SmbPort, ScanTimeoutMs).IsPortOpen();
        _hosts[host].IsScanning = false;
        return isPortOpen;
    }

    private void OnSmbPortOpened(Host host)
    {
        if (!_hosts.ContainsKey(host)) return;
        if (_hosts[host].LastScanStatus == LastScanStatus.Accessible) return;

        _hosts[host].LastScanStatus = LastScanStatus.Accessible;

        LogWriteLine($"Доступен SMB-сервер по адресу {host.IPAddress}.");
        OnSmbAccessible?.Invoke(host);
    }

    private void OnSmbPortClosed(Host host)
    {
        if (!_hosts.ContainsKey(host)) return;
        if (_hosts[host].LastScanStatus == LastScanStatus.Unaccessible) return;

        _hosts[host].LastScanStatus = LastScanStatus.Unaccessible;

        LogWriteLine($"SMB-сервер по адресу {host.IPAddress} не доступен.");
        OnSmbUnaccessible?.Invoke(host);
    }
}