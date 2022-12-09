using System.ComponentModel;
using MahApps.Metro.Controls;

namespace Forms
{
    /// <summary>
    /// Логика взаимодействия для MainForm.xaml
    /// </summary>
    public partial class MainForm : MetroWindow
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_OnClosing(object? sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
