using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Win32;
using SMBMonitor.Annotations;
using SMBMonitor.Model;
using SmbMonitorLib;

namespace SMBMonitor.ViewModel;

public class SettingsViewModel :INotifyPropertyChanged
{
    private readonly SettingsModel _settingsModel;
    
    public event PropertyChangedEventHandler? PropertyChanged;

    public SettingsViewModel(SettingsModel settingsModel)
    {
        _settingsModel = settingsModel;
        LogFileName = _settingsModel.LogFilename;
        IsUnavailableSharesMonitoringAutostart = _settingsModel.AutostartUnavailableSharesMonitor;
        Timings = _settingsModel.Timings;
        SmbPort = _settingsModel.SmbPort;

        SelectLogFile = new RelayCommand(_ => SelectLogFileProc());
        CloseWindow = new RelayCommand(_ => _settingsModel.SaveSettingsToFile());
    }

    public RelayCommand SelectLogFile { get; }

    public RelayCommand CloseWindow { get; }

    public string LogFileName
    {
        get => _settingsModel.LogFilename;
        set => _settingsModel.LogFilename = value;
    }

    public Timings Timings
    {
        get => _settingsModel.Timings;
        set => _settingsModel.Timings = value;
    }

    public int SmbPort
    {
        get => _settingsModel.SmbPort;
        set => _settingsModel.SmbPort = value;
    }

    public bool IsSaveLogToFile
    {
        get => _settingsModel.SaveLogToFile;
        set => _settingsModel.SaveLogToFile = value;
    }

    public bool IsUnavailableSharesMonitoringAutostart
    {
        get => _settingsModel.AutostartUnavailableSharesMonitor;
        set => _settingsModel.AutostartUnavailableSharesMonitor = value;
    }

    private void SelectLogFileProc()
    {
        var openFileDialog = new OpenFileDialog
        {
            AddExtension = true,
            Multiselect = false,
            Title = "Выберете файл для сохранения логов",
            DefaultExt = "log",
            Filter = "Файлы логов (*.log)|*.log|Все файлы (*.*)|*.*",
            CheckFileExists = false
        };

        if (openFileDialog.ShowDialog() ?? false)
        {
            LogFileName = openFileDialog.FileName;
        }
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
