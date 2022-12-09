using System.Net;
using NetworkUtils;
using SmbMonitorLib.Interfaces;
using SmbMonitorLib.Services.Base;

namespace SmbMonitorLib.Services;

internal class HostMonitoringService : ControlledService<HostMonitoringService>, IHostMonitoringService
{
    private readonly ISettings _settings;
    private readonly IStorageService _storage;
    private Timer? _pollingTimer;

    public HostMonitoringService(IStorageService storageService, ISettings settings)
    {
        _storage = storageService;
        _settings = settings;
        Logger = _settings.Logger;
    }

    protected override void OnStart()
    {
        void TimerCallbackProc(object? args)
        {
            var list = _storage.MonitorNodes
                .Where(item =>
                    !item.Key.Host.IPAddress.Equals(IPAddress.None)
                    && item.Value.IsScanning == false);

            Parallel.ForEach(list, ScanTask);
        }

        _pollingTimer = new Timer
        (
            TimerCallbackProc,
            null,
            0,
            _settings.PollingIntervalMs
        );
    }

    protected override void OnStop()
    {
        _pollingTimer?.Dispose();
        _pollingTimer = null;
    }

    private async void ScanTask(KeyValuePair<MonitorNode, MonitorNodeInfo> node)
    {
        var address = node.Key.Host.IPAddress;

        node.Value.IsScanning = true;
        var isSmbAccessible = await IsSmbServerAccessible(address);
        node.Value.IsScanning = false;
        
        if (isSmbAccessible)
        {
            if (node.Value.SmbServerStatus == SmbServerStatus.Connected ) return;

            node.Value.SmbCheckTries = 0;
            LogWriteLine($"Доступен SMB-сервер по адресу {address}.");
            node.Value.SmbServerStatus = SmbServerStatus.Connected;
        }
        else
        {
            if (node.Value.SmbServerStatus == SmbServerStatus.Disconnected) return;
            if (node.Value.SmbCheckTries <= _settings.TriesToUnaccessible)
            {
                node.Value.SmbCheckTries += 1;
            }
            else
            {
                LogWriteLine($"SMB-сервер по адресу {address} не доступен.");
                node.Value.SmbServerStatus = SmbServerStatus.Disconnected;
            }

        }
    }

    private async Task<bool> IsSmbServerAccessible(IPAddress host)
    {
        var result = false;
        var cancelTokenSource = new CancellationTokenSource();
        var cancelToken = cancelTokenSource.Token;

        var smb1Checker = new PortChecker(host, _settings.Smb1Port, _settings.ScanTimeoutMs);
        var smb2Checker = new PortChecker(host, _settings.Smb2Port, _settings.ScanTimeoutMs);

        var smb1PortCheckTask = Task.Run(() => smb1Checker.IsPortOpenAsync(cancelToken), cancelToken);
        var smb2PortCheckTask = Task.Run(() => smb2Checker.IsPortOpenAsync(cancelToken), cancelToken);

        var taskList = new List<Task<bool>> { smb1PortCheckTask, smb2PortCheckTask };
        while (taskList.Any())
        {
            var completedTask = await Task.WhenAny(taskList);
            result = completedTask.Result;

            if (result)
            {
                cancelTokenSource.Cancel();
                taskList.Clear();
            }
            else
            {
                taskList.Remove(completedTask);
            }
        }

        return result;
    }
}