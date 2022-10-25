using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using SMBMonitor.Annotations;
using SMBMonitor.Exceptions;
using SmbMonitorLib;

namespace SMBMonitor.Model;

//TODO Добавить систему логов
public class MonitorsModel
{

    private int _smbPort;
    private Timings _monitoringTimings;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<SmbServerMonitor> MonitorsList { get; }

    public MonitorsModel()
    {
        MonitorsList = new ObservableCollection<SmbServerMonitor>();
        ModelsContainer.Instance.SettingsModel.PropertyChanged += (_, _) => SyncSettings();
        SyncSettings();
    }

    #region Interface

    public int SmbPort
    {
        get => _smbPort;
        set
        {
            if (value == _smbPort) return;

            _smbPort = value;
            foreach (var monitor in MonitorsList)
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
            foreach (var monitor in MonitorsList)
            {
                monitor.Timings = _monitoringTimings;
            }
        }
    }

    public void StartMonitorByIndex(int index)
    { 
        if(index < 0 && index > MonitorsList.Count - 1)
            throw new ArgumentOutOfRangeException(nameof(index));

        MonitorsList[index].StartMonitoring();
    }

    public void StopMonitorByIndex(int index)
    {
        if(index < 0 && index > MonitorsList.Count - 1)
            throw new ArgumentOutOfRangeException(nameof(index));

        MonitorsList[index].StopMonitoring();
    }

    public void AddMonitor(MonitoringPoint mp)
    {
        if (IsMonitorExists(mp))
            throw new DuplicateException($"Монитор {mp.MonitoringObject} уже существует");

        var monitor = new SmbServerMonitor(mp);
        monitor.SmbPort = SmbPort;
        monitor.Timings = MonitoringTimings;
        MonitorsList.Add(monitor);
    }

    public void DeleteMonitorByIndex(int index)
    {
        if(index < 0 && index > MonitorsList.Count - 1)
            throw new ArgumentOutOfRangeException(nameof(index));
        
        var monitor = MonitorsList[index];
        monitor.StopMonitoring();
        MonitorsList.Remove(monitor);
        monitor.Dispose();
    }
    
    public void SaveMonitorsToFile()
    {
        var filename = CreateMonitorsListFilename();
        var jsonData = MonitorsListToJson();
        try
        {
            using var sw = new StreamWriter(filename, false);
            sw.Write(jsonData);
        }
        catch (Exception e)
        {
            throw new MonitorsSaveException(e.Message);
        }
    }

    public void LoadMonitorsFromFile()
    {
        var filename = CreateMonitorsListFilename();
        var jsonData = "";
        try
        {
            using var sr = new StreamReader(filename);
            jsonData = sr.ReadToEnd();
        }
        catch(FileNotFoundException)
        {
            
        }
        finally
        {
            JsonToMonitorsList(jsonData);
        }
    }
    #endregion
    
    #region Privates

    [MemberNotNull(nameof(_monitoringTimings))]
    private void SyncSettings()
    {
        SmbPort = ModelsContainer.Instance.SettingsModel.SmbPort;
        _monitoringTimings = ModelsContainer.Instance.SettingsModel.Timings;
    }

    private void JsonToMonitorsList(string jsonData)
    {
        if (jsonData == string.Empty) return;

        var monitoringPoints = JsonConvert.DeserializeObject<List<MonitoringPoint>>(jsonData)
                           ?? new List<MonitoringPoint>();

        foreach (var point in monitoringPoints)
        {
            var monitor = new SmbServerMonitor(point)
            {
                Timings = MonitoringTimings,
                SmbPort = SmbPort
            };
            MonitorsList.Add(monitor);
        }
    }

    private string MonitorsListToJson()
    {
        var list = GetMonitoringPointsFromMonitorsList();
        var json = JsonConvert.SerializeObject(list, Formatting.Indented);
        return json;
    }

    private List<MonitoringPoint> GetMonitoringPointsFromMonitorsList()
    {
        var list = MonitorsList.Select(monitor => monitor.MonitoringPoint).ToList();
        return list;
    }

    private string CreateMonitorsListFilename()
    {
        var workingDir = Defaults.CurrentAppDir;
        var containerFileName = Defaults.MonitorsListFileName;
        return $"{workingDir}\\{containerFileName}";
    }

    private bool IsMonitorExists(MonitoringPoint mp)
    {
        return MonitorsList.Any(mon => mon.Equals(mp));
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}
