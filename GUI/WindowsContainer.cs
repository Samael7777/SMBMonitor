using SMBMonitor.View;
using SMBMonitor.ViewModel;

namespace SMBMonitor;

public static class WindowsContainer
{
    private static MainWindow? mainWindow;
    private static SettingsWindow? settingsWindow;

    public static MainWindow MainWindow
    {
        get
        {
            if (mainWindow != null) return mainWindow;

            mainWindow ??= new MainWindow
            {
                DataContext = new MainViewModel(ModelsContainer.MainModel)
            };
            mainWindow.Closed += (_,_) => { mainWindow = null; };

            return mainWindow;
        }
    }

    public static SettingsWindow SettingsWindow
    {
        get
        {
            if (settingsWindow != null) return settingsWindow;

            settingsWindow ??= new SettingsWindow
            {
                DataContext = new SettingsViewModel(ModelsContainer.SettingsModel)
            };
            settingsWindow.Closed += (_,_) => { settingsWindow = null; };

            return settingsWindow;
        }
    }
}
