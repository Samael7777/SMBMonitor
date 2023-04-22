using System.Windows;
using SmbMonitor.App.ViewModels;
using SmbMonitor.App.Views;
using SmbMonitor.Base.DTO;

namespace SmbMonitor.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Settings _settings;
        private readonly SettingsView _settingsView;

        public App()
        {
            InitializeComponent();

            _settings = new Settings();
            _settings.SetDefault();

            _settingsView = new SettingsView
            {
                DataContext = new SettingsViewModel(_settings)
            };
        }
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainView = new MainView();
            mainView.Show();
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            
        }
    }

}
