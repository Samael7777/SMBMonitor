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
public class MainModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public MainModel()
    {
        ServerMonitors = new ObservableCollection<SmbServerMonitor>();
        SmbPort = Defaults.SmbPort;
        MonitoringTimings = Defaults.Timings;
    }

    public ObservableCollection<SmbServerMonitor> ServerMonitors { get; }
    public int SmbPort { get; set; }

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

    public Timings MonitoringTimings { get; set; }

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

    public void SaveList()
    {
        throw new NotImplementedException();
    }

    public void LoadList()
    {
        throw new NotImplementedException();
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
