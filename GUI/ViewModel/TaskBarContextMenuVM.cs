using System.Windows;

namespace SMBMonitor.ViewModel;

public class TaskBarContextMenuVM
{
    public TaskBarContextMenuVM()
    {
        ShowMainWindow = new RelayCommand(_ => WindowsContainer.MainWindow.Show());
        ShowSettingsWindow = new RelayCommand(_ => WindowsContainer.SettingsWindow.Show());
        ShowAboutWindow = new RelayCommand(_ => { });
        ExitApplication = new RelayCommand(_ => Application.Current.Shutdown());
    }

    public RelayCommand ShowMainWindow { get; }
    public RelayCommand ShowSettingsWindow { get; }
    public RelayCommand ShowAboutWindow { get; }
    public RelayCommand ExitApplication { get; }
}
