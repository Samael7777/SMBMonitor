using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using SMBMonitor.Model;
using SMBMonitor.View;
using SMBMonitor.ViewModel;

namespace SMBMonitor
{
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
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            _taskbarIcon.ToolTipText = "SMB Servers Monitor";
            _taskbarIcon.Visibility = Visibility.Visible;

            var mainModel = new MainModel();
            var mainWindow = new MainWindow();
            var mainVM = new MainVM(mainModel, mainWindow);
            
            mainWindow.MonitorsList.SelectionChanged += mainVM.OnSelectionChanged;
            
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _taskbarIcon.Visibility = Visibility.Collapsed;
            _taskbarIcon.Dispose();
            
            base.OnExit(e);
        }
    }
}
