using CommunityToolkit.Mvvm.ComponentModel;
using SmbMonitor.Base;
using SmbMonitor.Base.DTO;

namespace SmbMonitor.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        _settingsVm = new SettingsViewModel(new Settings().SetDefault());
    }

    [ObservableProperty] private SettingsViewModel _settingsVm;
}
