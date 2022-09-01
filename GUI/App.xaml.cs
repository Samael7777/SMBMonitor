using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using SMBMonitor.ViewModel;


namespace SMBMonitor;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private readonly TaskbarIcon _taskbarIcon;

    public App()
    {
        InitializeComponent();
        _taskbarIcon = (TaskbarIcon)Resources["NotifyIcon"];
        var contextMenu = (ContextMenu)Resources["TaskBarContextMenu"];
        contextMenu.DataContext =new TaskBarContextMenuVM();
        _taskbarIcon.ContextMenu = contextMenu;
    }
    protected override void OnStartup(StartupEventArgs e)
    {
        _taskbarIcon.ToolTipText = "SMB Servers Monitor";
        _taskbarIcon.Visibility = Visibility.Visible;
        
        WindowsContainer.MainWindow.Show();

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _taskbarIcon.Visibility = Visibility.Collapsed;
        _taskbarIcon.Dispose();
        
        base.OnExit(e);
    }
}

