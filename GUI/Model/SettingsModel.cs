using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Newtonsoft.Json;
using SMBMonitor.Annotations;
using SmbMonitorLib;

namespace SMBMonitor.Model;

//TODO Добавить логгирование
public class SettingsModel : INotifyPropertyChanged
{
    private readonly string _appDir;
    private string _settingsFile;
    private SettingsContainer _container;
    private string _jsonString;
    
    public event PropertyChangedEventHandler? PropertyChanged;

    public Timings Timings
    {
        get => _container.Timings;
        set
        {
            _container.Timings = value;
            OnSettingChanged(nameof(Timings));
        }
    }
    
    public int SmbPort
    {
        get => _container.SmbPort;
        set
        {
            _container.SmbPort = value;
            OnSettingChanged(nameof(SmbPort));
        }
    }
    
    public bool AutostartUnavailableSharesMonitor
    {
        get => _container.AutostartUnavailableSharesMonitor;
        set
        {
            _container.AutostartUnavailableSharesMonitor = value;
            OnSettingChanged(nameof(AutostartUnavailableSharesMonitor));
        }
    }
    
    public bool SaveLogToFile
    {
        get => _container.SaveLogToFile;
        set
        {
            _container.SaveLogToFile = value;
            OnSettingChanged(nameof(SaveLogToFile));
        }
    }

    public string LogFilename
    {
        get => _container.LogFilename;
        set
        {
            _container.LogFilename = value;
            OnSettingChanged();
        }
    }

    public SettingsModel()
    {
        _container = new SettingsContainer();
        _jsonString = "";
        _settingsFile = "";
        _appDir = GetCurrentAppDir();
        SetDefaultSettings();
        LoadSettingsFromFile();
    }

    private void SetDefaultSettings()
    {
        _container.AutostartUnavailableSharesMonitor = Defaults.AutostartUnavailableSharesMonitor;
        _container.SaveLogToFile = Defaults.SaveLogToFile;
        _container.Timings = Defaults.Timings;
        _container.SmbPort = Defaults.SmbPort;

        var logFileName = Defaults.LogFileName;
        _container.LogFilename = $"{_appDir}\\{logFileName}";
        
        var settingsFileName = Defaults.SettingsFileName;
        _settingsFile = $"{_appDir}\\{settingsFileName}";
    }

    private static string GetCurrentAppDir()
    {
        var appPath = Path.GetDirectoryName
            (Process.GetCurrentProcess().MainModule?.FileName) ?? "";
        return appPath;
    }

    public void SaveSettingsToFile()
    {
        try
        {
            _jsonString = JsonConvert.SerializeObject(_container);
            using var sw = new StreamWriter(_settingsFile);
            sw.Write(_jsonString);
        }
        catch(Exception e)
        {
            MessageBox.Show(e.Message, "Ошибка сохранения настроек!",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }

    public void LoadSettingsFromFile()
    {
        try
        {
            using var sr = new StreamReader(_settingsFile);
            
            _jsonString = sr.ReadToEnd();
            _container = JsonConvert.DeserializeObject<SettingsContainer>(_jsonString) 
                         ?? new SettingsContainer();
        }
        catch
        {
            MessageBox.Show("Настройки не найдены, используются настройки по умолчанию", "Внимание!",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    

    [NotifyPropertyChangedInvocator]
    protected virtual void OnSettingChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
