using System.Windows;

namespace SMBMonitor.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectAll(object sender, RoutedEventArgs e)
        {
            MonitorsList.SelectAll();
        }

        private void SelectNone(object sender, RoutedEventArgs e)
        {
            MonitorsList.UnselectAll();
        }
    }
}
