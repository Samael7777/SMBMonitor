using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SMBMonitor.Annotations;
using SmbMonitorLib;


namespace SMBMonitor.ViewModel;

public class MonitorListItemVM : INotifyPropertyChanged, IEquatable<SmbServerMonitor>, IDisposable
{
    private readonly ResourceDictionary _resources;
    private BitmapImage _elementConnectedImage;
    private BitmapImage _elementDisconnectedImage;
    private BitmapImage _typeImage;
    private BitmapImage _stateImage;
    private Brush _monitorStatusColor;
    private string _sharesConnectedString;
    private string _title;
    
    public event PropertyChangedEventHandler? PropertyChanged;

    public SmbServerMonitor LinkedMonitor { get; }


    #region Bindings
    public BitmapImage TypeImage
    {
        get => _typeImage;
        set
        {
            _typeImage = value;
            OnPropertyChanged(nameof(TypeImage));
        }
    }
    public Brush MonitorStatusColor
    {
        get => _monitorStatusColor;
        set
        {
            _monitorStatusColor = value;
            MonitorStatusColor.Freeze();
            OnPropertyChanged(nameof(MonitorStatusColor));
        }
    }
    public string SharesConnectedString
    {
        get => _sharesConnectedString;
        set
        {
            _sharesConnectedString = value;
            OnPropertyChanged(nameof(SharesConnectedString));
        }
    }
    public BitmapImage StateImage
    {
        get => _stateImage;
        set
        {
            _stateImage = value;
            StateImage.Freeze();
            OnPropertyChanged(nameof(StateImage));
        }
    }
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged(nameof(Title));
        }
    } 
    #endregion

    public MonitorListItemVM(SmbServerMonitor monitor)
    {
        _resources = Application.Current.Resources;
        _elementConnectedImage = new BitmapImage();
        _elementDisconnectedImage = new BitmapImage();
        _typeImage = new BitmapImage();
        _stateImage = new BitmapImage();
        _monitorStatusColor = new SolidColorBrush();
        _sharesConnectedString = "";
        _title = "";
        
        LinkedMonitor = monitor;
        Title = monitor.Description;
        monitor.OnStatusChanged += OnMonitorStatusChange;
        SetImagesGroup(monitor.Type);
        OnMonitorStatusChange(monitor);
    }

    private void UpdateElement(SmbServerMonitor monitor)
    {
        SetMonitorStatusColor(monitor.IsRunning);
        SetConnectionStatus(monitor.IsAccessible);
        SetSharesConnectedString(monitor.Shares.Count);
    }

    private void OnMonitorStatusChange(SmbServerMonitor monitor)
    {
        UpdateElement(monitor);
    }

    private void SetSharesConnectedString(int sharesCount)
    {
        SharesConnectedString = sharesCount switch
        {
            0 => "Нет подключенных дисков", 
            > 0 => $"Подключено дисков: {sharesCount}",
            < 0 => "Ошибка!"
        };
    }

    private void SetConnectionStatus(bool isConnected)
    {
        StateImage = isConnected
            ? _elementConnectedImage
            : _elementDisconnectedImage;
    }

    private void SetMonitorStatusColor(bool isRunning)
    {
        MonitorStatusColor = isRunning
            ? new SolidColorBrush(Colors.Green) 
            : new SolidColorBrush(Colors.Red);
    }

    private void SetImagesGroup(ServerType type)
    {
        switch (type)
        {
            case ServerType.AccessPoint:
                SetWifiImagesGroup();
                break;

            case ServerType.Host:
                SetWiredImagesGroup();
                break;

            default:
                SetUnavailableImagesGroup();
                break;
        }
    }

    private void SetWifiImagesGroup()
    {
       _elementConnectedImage = (BitmapImage)_resources["WifiConnected"];
       _elementDisconnectedImage = (BitmapImage)_resources["WifiDisconnected"];
       TypeImage = (BitmapImage)_resources["WifiLogo"];
    }

    private void SetWiredImagesGroup()
    {
       _elementConnectedImage = (BitmapImage)_resources["WiredConnected"];
       _elementDisconnectedImage = (BitmapImage)_resources["WiredDisconnected"];
       TypeImage = (BitmapImage)_resources["WiredLogo"];
    }

    private void SetUnavailableImagesGroup()
    {
        _elementConnectedImage = (BitmapImage)_resources["Unavailable"];
        _elementDisconnectedImage = (BitmapImage)_resources["Unavailable"];
        TypeImage = (BitmapImage)_resources["Unavailable"];
    }

   

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #region Equasls
    protected bool Equals(MonitorListItemVM other)
    {
        return LinkedMonitor.Equals(other.LinkedMonitor);
    }

    public bool Equals(SmbServerMonitor? other)
    {
        return other != null && LinkedMonitor.Equals(other);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (ReferenceEquals(this, obj)) return true;

        if (obj.GetType() == LinkedMonitor.GetType()) return LinkedMonitor.Equals(obj);

        return obj.GetType() == GetType() && Equals((MonitorListItemVM)obj);
    }

    public override int GetHashCode()
    {
        return LinkedMonitor.GetHashCode();
    } 
    #endregion

    #region Dispose

    private bool _disposed;

    ~MonitorListItemVM()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            //dispose managed state (managed objects)
            LinkedMonitor.OnStatusChanged -= OnMonitorStatusChange;
        }
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null
        _disposed = true;
    }

    #endregion
}