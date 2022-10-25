using System.Windows;

namespace SMBMonitor.ViewModel;

//TODO Настроить отображение окон
public class TaskBarContextMenuViewModel
{
    public TaskBarContextMenuViewModel()
    {
        ShowMainWindow = new RelayCommand(_ => { });
        ShowSettingsWindow = new RelayCommand(_ => { });
        ShowAboutWindow = new RelayCommand(_ => { });
        ExitApplication = new RelayCommand(_ => Application.Current.Shutdown());
    }

    public RelayCommand ShowMainWindow { get; }
    public RelayCommand ShowSettingsWindow { get; }
    public RelayCommand ShowAboutWindow { get; }
    public RelayCommand ExitApplication { get; }
}
