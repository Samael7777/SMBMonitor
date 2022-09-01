using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using SMBMonitor.Annotations;
using SMBMonitor.Exceptions;
using SmbMonitorLib;

namespace SMBMonitor.Model;

//TODO Добавить систему логов
public class MainModel
{
    private readonly SettingsModel _settingsModel;
    private int _smbPort;
    private Timings _monitoringTimings;

    public event PropertyChangedEventHandler? PropertyChanged;

    public MainModel(SettingsModel settingsModel)
    {
        _monitoringTimings = new Timings();
        ServerMonitors = new ObservableCollection<SmbServerMonitor>();
        _settingsModel = settingsModel;
        _settingsModel.PropertyChanged += OnSettingsChanged;
        GetSettings();
    }

    public ObservableCollection<SmbServerMonitor> ServerMonitors { get; }

    public int SmbPort
    {
        get => _smbPort;
        set
        {
            if (value == _smbPort) return;

            _smbPort = value;
            foreach (var monitor in ServerMonitors)
            {
                monitor.SmbPort = _smbPort;
            }
        }
    }

    public Timings MonitoringTimings
    {
        get => _monitoringTimings;
        set
        {
            if (_monitoringTimings.Equals(value)) return;

            _monitoringTimings = value;
            foreach (var monitor in ServerMonitors)
            {
                monitor.Timings = _monitoringTimings;
            }
        }
    }

    public void StartSelected(IEnumerable<int> selectedItems)
    {
        foreach (var item in selectedItems)
        {
            ServerMonitors[item].StartMonitoring();
        }
    }

    public void StopSelected(IEnumerable<int> selectedItems)
    {
        foreach (var item in selectedItems)
        {
            ServerMonitors[item].StopMonitoring();
        }
    }

    public void AddMonitor(MonitoringPoint mp)
    {
        if (IsMonitorExists(mp))
            throw new DuplicateException($"Монитор {mp.MonitoringObject} уже существует");

        var monitor = new SmbServerMonitor(mp);
        monitor.SmbPort = SmbPort;
        monitor.Timings = MonitoringTimings;
        ServerMonitors.Add(monitor);
    }

    public void RemoveMonitorByIndex(int index)
    {
        var monitor = ServerMonitors[index];
        monitor.StopMonitoring();
        monitor.Dispose();
        ServerMonitors.RemoveAt(index);
    }

    public void OnSettingsChanged(object? sender, PropertyChangedEventArgs args)
    {
        GetSettings();
    }

    public void SaveList()
    {
        throw new NotImplementedException();
    }

    public void LoadList()
    {
        throw new NotImplementedException();
    }
    
    private void GetSettings()
    {
        SmbPort = _settingsModel.SmbPort;
        MonitoringTimings = _settingsModel.Timings;
    }

    private bool IsMonitorExists(MonitoringPoint mp)
    {
        return ServerMonitors.Any(mon => mon.Equals(mp.MonitoringObject));
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
