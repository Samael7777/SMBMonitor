
namespace SMBMonitor.ViewModel;

public class MainViewModel : BaseViewModel
{
    public MainViewModel()
    {
        SettingsCommand = new RelayCommand(_ => ShowSettings());
    }
    
    public RelayCommand SettingsCommand { get; private set; }

    private void ShowSettings()
    {
        
    }
}
