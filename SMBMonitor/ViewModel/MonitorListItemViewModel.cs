using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using SMBMonitor.Annotations;
using SmbMonitorLib;
using MahApps.Metro.IconPacks;

namespace SMBMonitor.ViewModel;

public class MonitorListItemViewModel : INotifyPropertyChanged, IDisposable, IEquatable<SmbServerMonitor>
{
    private PackIconControlBase _isConnectedImage;
    private PackIconControlBase _isDisconnectedImage;
    private PackIconControlBase _stateImage;

    private Brush _monitorStatusColor;
    private string _sharesConnectedString;

    public event PropertyChangedEventHandler? PropertyChanged;

    public SmbServerMonitor LinkedMonitor { get; }

    public MonitorListItemViewModel(SmbServerMonitor monitor)
    {
        void OnMonitorStatusChanged(SmbServerMonitor mon)
        {
            Application.Current.Dispatcher.Invoke(() => UpdateElements(mon));
        }
        monitor.OnStatusChanged += OnMonitorStatusChanged;

        LinkedMonitor = monitor;
        Title = monitor.Description;
        SetImagesGroup(monitor.Type);
        SetImagesSize48x48();

        UpdateElements(monitor);
    }

    #region Bindings

    public PackIconControlBase LogoImage { get; private set; }

    public PackIconControlBase StateImage
    {
        get => _stateImage;

        [MemberNotNull(nameof(_stateImage))]
        set
        {
            _stateImage = value;
            OnPropertyChanged(nameof(StateImage));
        }
    }

    public Brush MonitorStatusColor
    {
        get => _monitorStatusColor;
        
        [MemberNotNull(nameof(_monitorStatusColor))]
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

        [MemberNotNull(nameof(_sharesConnectedString))]
        set
        {
            _sharesConnectedString = value;
            OnPropertyChanged(nameof(SharesConnectedString));
        }
    }
    
    public string Title { get; }

    #endregion

    [MemberNotNull(nameof(_stateImage), nameof(_sharesConnectedString), nameof(_monitorStatusColor))]
    private void UpdateElements(SmbServerMonitor monitor)
    {
        SetMonitorStatusColor(monitor.IsRunning);
        SetConnectionStatus(monitor.IsAccessible);
        SetSharesConnectedString(monitor.Shares.Count);
    }

    [MemberNotNull(nameof(_sharesConnectedString))]
    private void SetSharesConnectedString(int sharesCount)
    {
        SharesConnectedString = sharesCount switch
        {
            0 => "Нет подключенных дисков", 
            > 0 => $"Подключено дисков: {sharesCount}",
            < 0 => "Ошибка!"
        };
    }

    [MemberNotNull(nameof(_stateImage))]
    private void SetConnectionStatus(bool isConnected)
    {
        StateImage = isConnected
            ? _isConnectedImage
            : _isDisconnectedImage;
    }

    [MemberNotNull(nameof(_monitorStatusColor))]
    private void SetMonitorStatusColor(bool isRunning)
    {
        MonitorStatusColor = isRunning
            ? new SolidColorBrush(Colors.Green) 
            : new SolidColorBrush(Colors.Red);
    }

    [MemberNotNull(nameof(_isConnectedImage), nameof(_isDisconnectedImage), nameof(LogoImage))]
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
                throw new ArgumentException("Неподдерживаемый тип монитора");
        }
    }
    
    [MemberNotNull(nameof(_isConnectedImage), nameof(_isDisconnectedImage), nameof(LogoImage))]
    private void SetWifiImagesGroup()
    {
        LogoImage = new PackIconFontisto
        {
            Kind = PackIconFontistoKind.WifiLogo
        };

        _isDisconnectedImage = new PackIconEvaIcons
        {
            Kind = PackIconEvaIconsKind.WifiOff,
        };

        _isConnectedImage = new PackIconEvaIcons
        {
            Kind = PackIconEvaIconsKind.Wifi
        };
    }

    [MemberNotNull(nameof(_isConnectedImage), nameof(_isDisconnectedImage), nameof(LogoImage))]
    private void SetWiredImagesGroup()
    {
        LogoImage = new PackIconMaterial()
        {
            Kind = PackIconMaterialKind.Lan
        };

        _isDisconnectedImage = new PackIconMaterial
        {
            Kind = PackIconMaterialKind.LanConnect
        };

        _isConnectedImage = new PackIconMaterial
        {
            Kind = PackIconMaterialKind.LanDisconnect
        };
    }

    private void SetImagesSize48x48()
    {
        SetElementSize48x48(LogoImage);
        SetElementSize48x48(_isConnectedImage);
        SetElementSize48x48(_isDisconnectedImage);
    }

    private void SetElementSize48x48(FrameworkElement element)
    {
        element.Height = 48;
        element.Width = 48;
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    #region Equals
    protected bool Equals(MonitorListItemViewModel other)
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
   
        return obj.GetType() == GetType() && Equals((MonitorListItemViewModel)obj);
    }
   
    public override int GetHashCode()
    {
        return LinkedMonitor.GetHashCode();
    } 
    #endregion

    #region Dispose

    private bool _disposed;

    ~MonitorListItemViewModel()
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
            LinkedMonitor.OnStatusChanged -= UpdateElements;
        }
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null
        _disposed = true;
    }

    #endregion
}