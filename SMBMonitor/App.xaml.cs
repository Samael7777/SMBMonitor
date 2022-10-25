using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Toolkit.Uwp.Notifications;
using SMBMonitor.View;
using SMBMonitor.ViewModel;


namespace SMBMonitor;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private TaskbarIcon _taskbarIcon;
    private MainWindow _mainWindow;

    public App()
    {
        InitializeComponent();
        InitializeModels();
        InitializeMainWindow();
        InitializeCommands();
        InitializeTaskbarIcon();
    }

    public RelayCommand ShowMainWindow { get; private set; }

    private static void InitializeModels()
    {
        ModelsContainer.Instance.BuildDependencies();
    }

    [MemberNotNull(nameof(_mainWindow))]
    private void InitializeMainWindow()
    {
        _mainWindow = new MainWindow();
    }

    [MemberNotNull(nameof(ShowMainWindow))]
    private void InitializeCommands()
    {
        ShowMainWindow = new RelayCommand(_ => _mainWindow.Show());
    }


    [MemberNotNull(nameof(_taskbarIcon))]
    private void InitializeTaskbarIcon()
    {
        _taskbarIcon = (TaskbarIcon)Resources["NotifyIcon"];
        var contextMenu = (ContextMenu)Resources["TaskBarContextMenu"];
        contextMenu.DataContext =new TaskBarContextMenuViewModel();
        _taskbarIcon.ContextMenu = contextMenu;
        _taskbarIcon.DoubleClickCommand = ShowMainWindow;
    }
    
    protected override void OnStartup(StartupEventArgs e)
    {
        CreateTaskbarIcon();
        LoadAppData();

        _mainWindow.Show();

        base.OnStartup(e);
    }
    
    protected override void OnExit(ExitEventArgs e)
    {
        SaveAppData();
        DeleteTaskbarIcon();
        
        base.OnExit(e);
    }

    private void LoadAppData()
    {
        LoadMonitorsList();
        LoadAppSettings();
    }

    private static void SaveAppData()
    {
        SaveMonitorsList();
        SaveAppSettings();
    }

    private static void LoadMonitorsList()
    {
        ModelsContainer.Instance.MonitorsModel.LoadMonitorsFromFile();
    }
    
    private static void SaveMonitorsList()
    {
        ModelsContainer.Instance.MonitorsModel.SaveMonitorsToFile();
    }

    private static void LoadAppSettings()
    {
        try
        {
            ModelsContainer.Instance.SettingsModel.LoadSettingsFromFile();
        }
        catch
        {
            const string message = "Ошибка загрузки настроек. Применены настройки по умолчанию.";
            new ToastContentBuilder()
                .AddText(message)
                .Show();
        }
    }
    
    private static void SaveAppSettings()
    {
        ModelsContainer.Instance.SettingsModel.SaveSettingsToFile();
    }
    private void CreateTaskbarIcon()
    {
        _taskbarIcon.ToolTipText = "SMB Servers Monitor";
        _taskbarIcon.Visibility = Visibility.Visible;
    }

    private void DeleteTaskbarIcon()
    {
        _taskbarIcon.Visibility = Visibility.Collapsed;
        _taskbarIcon.Dispose();
    }
}

