using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using SMBMonitor;
using SmbMonitorLib;
using SMBMonitor.Annotations;
using SMBMonitor.Exceptions;

namespace SMBMonitor.Model;

//TODO Добавить систему логов
public class SettingsModel : INotifyPropertyChanged
{
    private SettingsContainer _settingsContainer;
    
    public event PropertyChangedEventHandler? PropertyChanged;

    public SettingsModel()
    {
        InitializeSettings();
    }

    public Timings Timings
    {
        get => _settingsContainer.Timings;
        set
        {
            _settingsContainer.Timings = value;
            OnSettingChanged(nameof(Timings));
        }
    }
    
    public int SmbPort
    {
        get => _settingsContainer.SmbPort;
        set
        {
            _settingsContainer.SmbPort = value;
            OnSettingChanged(nameof(SmbPort));
        }
    }
    
    public bool AutostartUnavailableSharesMonitor
    {
        get => _settingsContainer.AutostartUnavailableSharesMonitor;
        set
        {
            _settingsContainer.AutostartUnavailableSharesMonitor = value;
            OnSettingChanged(nameof(AutostartUnavailableSharesMonitor));
        }
    }
    
    public bool SaveLogToFile
    {
        get => _settingsContainer.SaveLogToFile;
        set
        {
            _settingsContainer.SaveLogToFile = value;
            OnSettingChanged(nameof(SaveLogToFile));
        }
    }

    public string LogFilename
    {
        get => _settingsContainer.LogFilename;
        set
        {
            _settingsContainer.LogFilename = value;
            OnSettingChanged();
        }
    }

    public void SaveSettingsToFile()
    {
        var settingsFile = GetSettingsFileName();
        var jsonData = SettingsToJson();
        try
        {
            using var sw = new StreamWriter(settingsFile, false);
            sw.Write(jsonData);
        }
        catch(Exception e)
        {
            throw new SettingsSaveException(e.Message);
        }

    }

    public void LoadSettingsFromFile()
    {
        var settingsFile = GetSettingsFileName();
        var jsonData = "";
        try
        {
            using var sr = new StreamReader(settingsFile);
            jsonData = sr.ReadToEnd();
        }
        catch (Exception e)
        {
            throw new SettingsLoadException(e.Message);
        }
        finally
        {
            JsonToSettings(jsonData);
        }
    }
    

    [MemberNotNull(nameof(_settingsContainer))]
    private void InitializeSettings()
    {
        var appDir = Defaults.CurrentAppDir;
        var logFileName = Defaults.LogFileName;
        
        _settingsContainer = new SettingsContainer
        {
            AutostartUnavailableSharesMonitor = Defaults.AutostartUnavailableSharesMonitor,
            SaveLogToFile = Defaults.SaveLogToFile,
            Timings = Defaults.Timings,
            SmbPort = Defaults.SmbPort,
            LogFilename = $"{appDir}\\{logFileName}"
        };
    }

    private string SettingsToJson()
    {
        var json = JsonConvert.SerializeObject(_settingsContainer);
        return json;
    }

    private void JsonToSettings(string jsonData)
    {
        if (jsonData == string.Empty) return;
            
        var settings = JsonConvert.DeserializeObject<SettingsContainer>(jsonData);
        if (settings != null)
            _settingsContainer = settings;
    }

    private static string GetSettingsFileName()
    {
        return $"{Defaults.CurrentAppDir}\\{Defaults.SettingsFileName}";
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnSettingChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
