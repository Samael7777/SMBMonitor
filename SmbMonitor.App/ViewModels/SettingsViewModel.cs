using CommunityToolkit.Mvvm.ComponentModel;
using SmbMonitor.Base.DTO;

namespace SmbMonitor.App.ViewModels;

public class SettingsViewModel : ObservableObject
{
    private readonly Settings _settings;

    public SettingsViewModel(Settings settings)
    {
        _settings = settings;
    }

    public int PollingIntervalMs
    {
        get => _settings.PollingIntervalMs;
        set => SetProperty(ref _settings.PollingIntervalMs, value);
    }

    public int ScanTimeoutMs
    {
        get => _settings.ScanTimeoutMs;
        set => SetProperty(ref _settings.ScanTimeoutMs, value);
    }
    public int SmbPort
    {
        get => _settings.SmbPort;
        set => SetProperty(ref _settings.SmbPort, value);
    }
    public int TriesToUnaccessible
    {
        get => _settings.TriesToUnaccessible;
        set => SetProperty(ref _settings.TriesToUnaccessible, value);
    }
    public int DnsCacheUpdateIntervalMs
    {
        get => _settings.DnsCacheUpdateIntervalMs;
        set => SetProperty(ref _settings.DnsCacheUpdateIntervalMs, value);
    }
    public bool MonitorUnmanagedShares
    {
        get => _settings.MonitorUnmanagedShares;
        set => SetProperty(ref _settings.MonitorUnmanagedShares, value);
    }
    public bool DisconnectSharesAfterMonitorRemoved
    {
        get => _settings.DisconnectSharesAfterMonitorRemoved;
        set => SetProperty(ref _settings.DisconnectSharesAfterMonitorRemoved, value);
    }
    public bool AutoConnectDisconnectedServerShares
    {
        get => _settings.AutoConnectDisconnectedServerShares;
        set => SetProperty(ref _settings.AutoConnectDisconnectedServerShares, value);
    }
}