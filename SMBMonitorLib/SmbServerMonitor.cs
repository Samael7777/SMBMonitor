using System.ComponentModel;
using System.Net;
using SmbMonitorLib.PortChecking;
using SmbMonitorLib.SMB;
using SmbMonitorLib.Wifi;

namespace SmbMonitorLib;

public class SmbServerMonitor : IDisposable, IEquatable<SmbServerMonitor>
{
    private static readonly object syncRoot = new ();

    private readonly object _hashObject;

    private WifiApMonitor? _wifiApMonitor;
    private Action _onStart;
    private Action _onStop;
    private PortCheckerOnTimer? _portScannerTimer;
    private Func<bool> _isRunning;
    private string _lastServerName;
    private ILogger? _logger;

    private SmbServerMonitor()
    {
        Description = "";

        Shares = new List<SharedDisk>();

        _isRunning = () => false;
        _onStart = () => { };
        _onStop = () => { };

        Timings = new Timings();

        IsAccessible = false;
        Credentials = new Credentials();
        Host = new Host();
        _lastServerName = string.Empty;
        _hashObject = Host;
    }

    public SmbServerMonitor(MonitoringPoint mp) : this()
    {
        _hashObject = mp.MonitoringObject;
        Credentials = mp.Credentials;

        switch (mp.MonitoringObject)
        {
            case WifiNetworkIdentifier ap:
                RegisterMonitorByAP(ap);
                break;
            case Host host:
                RegisterMonitorByHost(host);
                break;
            default:
                throw new ArgumentException("Неподдерживаемый тип монитора.");
        }
    }
    
    #region Properties

    public event Action<SmbServerMonitor>? OnStatusChanged;

    public string Description { get; private set; }
    public ServerType Type { get; private set; }
    public int SmbPort { get; set; }
    public Credentials Credentials { get; set; }
    public Host Host { get; private set; }
    public Timings Timings { get; set; }
    public bool IsRunning => _isRunning();
    public bool IsAccessible { get; private set; }
    public List<SharedDisk> Shares { get; private set; }

    #endregion

    #region Public methods
    
    public void SetLogger(ILogger logger)
    {
        _logger = logger;
        _logger.SetPrefix(Description);
    }

    public void StartMonitoring()
    {
        _logger?.WriteFormattedLine("Мониторинг запущен.");

        _onStart.Invoke();
        OnStatusChanged?.Invoke(this);
    }

    public void StopMonitoring()
    {
        _logger?.WriteFormattedLine("Мониторинг остановлен.");

        _onStop.Invoke();
        OnStatusChanged?.Invoke(this);
    } 

    #endregion

    #region Events

    private void OnSMBAvailable()
    {
        IsAccessible = true;
        _lastServerName = "\\\\" + Host.IP;

        _logger?.WriteFormattedLine($"SMB сервер доступен по пути \\\\{Host.Name}.");

        SharesGetAllAvailable();
        ConnectShares();
        
        OnStatusChanged?.Invoke(this);
    }

    private void OnSMBUnavailable()
    {
        IsAccessible = false;

        _logger?.WriteFormattedLine($"SMB сервер не доступен.");

        DisconnectShares();
        Shares.Clear();

        OnStatusChanged?.Invoke(this);
    }

    #endregion

    #region Privates

    private void RegisterMonitorByAP(WifiNetworkIdentifier ap)
    {
        void OnApConnected(IPAddress address)
        {
            Host = new Host(address, SmbPort);
            StartPortScanning();
        }

        void OnApDisconnected()
        {
            Host = new Host();
            StopPortScanning();
            OnSMBUnavailable();
        }

        Type = ServerType.AccessPoint;
        Description = $"Точка доступа {ap}";

        _wifiApMonitor = new WifiApMonitor(ap);
        _onStart = _wifiApMonitor.Start;
        _onStop = _wifiApMonitor.Stop;
        _isRunning = () => _wifiApMonitor.IsRunning;
        _wifiApMonitor.OnAPConnected = OnApConnected;
        _wifiApMonitor.OnAPDisconnected = OnApDisconnected;
    }

    private void RegisterMonitorByHost(Host host)
    {
        _onStart = () =>
        {
            if (IsRunning)
                return;

            _isRunning = () => true;
            StartPortScanning();
        };

        _onStop = () =>
        {
            if (!IsRunning)
                return;

            _isRunning = () => false;
            StopPortScanning();
        };

        Type = ServerType.Host;
        Description = $"Хост {host.Name}";

        _isRunning = () => false;
        Host = host;
    }

    private void SharesGetAllAvailable()
    {
        Shares.Clear();

        if (!IsAccessible || Host.IP.Equals(IPAddress.None))
            return;

        var serverName = $"\\\\{Host.IP}";
        var message = "";

        //TODO Иногда выпадает исключение - ресурс не доступен, что-то с этим сделать. Возможно, повтор запроса...
        try
        {
            Shares = SmbClient.GetRemoteShares(serverName, Credentials);
        }
        catch (Win32Exception e)
        {
            //if (e.NativeErrorCode != 0x35)  //Не найден сетевой путь
            //    throw;
            message = $"Ошибка получения списка ресурсов сервера {serverName} : " +
                      $"{e.Message}";
        }
        if (message != string.Empty)
            _logger?.WriteFormattedLine(message);
    }

    private void StartPortScanning()
    {
        if (_portScannerTimer != null)
            return;
       
        Host.Port = SmbPort;
        var settings = new CheckerSettings
        {
            Host = Host,
            Interval = Timings.Interval,
            Timeout = Timings.Timeout,
            OnPortAccessible = OnSMBAvailable,
            OnPortInaccessible = OnSMBUnavailable
        };
        _portScannerTimer = new PortCheckerOnTimer(settings);
    }
    
    private void StopPortScanning()
    {
        if(_portScannerTimer == null) 
            return;

        _portScannerTimer.Dispose();
        _portScannerTimer = null;
    }
    
    private void ConnectShares()
    {
        foreach (var share in Shares)
        {
           ConnectShare(share);
        }
    }

    private void DisconnectShares()
    {
        var connectedShares = SmbClient.GetConnectedShares();
       
        var readyToDisconnect = connectedShares
            .Where(share => share.RootPath
                    .Equals(_lastServerName, StringComparison.OrdinalIgnoreCase));
        
        foreach (var share in readyToDisconnect)
        {
            DisconnectShare(share);
        }

    }

    private void ConnectShare(SharedDisk share)
    {
        if (IsShareConnected(share))
            return;

        string message;

        lock (syncRoot)
        {
            try
            {
                var letter = DriveLettersManager.GetNextFreeDriveLetter();
                
                SmbClient.ConnectNetworkDisk(share.RemoteName, letter, Credentials);
                
                share.LocalName = $"{letter}:\\";
                message = $"Ресурс {share.RemoteName} подключен как диск {share.LocalName}";
            }
            catch (IndexOutOfRangeException)
            {
                message = $"Нет доступных букв дисков для подключения ресурса {share.RemoteName}";
            }
            catch (Win32Exception e)
            {
                message = $"Ошибка подключения ресурса {share.RemoteName} : {e.Message}";
            }
        }
        _logger?.WriteFormattedLine(message);
    }

    private void DisconnectShare(SharedDisk share)
    {
        if (share.LocalName == string.Empty)
            return;

        var message = $"{nameof(DisconnectShare)} : Неизвестная ошибка";

        var usingLetter = share.LocalName[0];
        try
        {
            SmbClient.DisconnectNetworkDisk(usingLetter);
            message = $"Ресурс {share.RemoteName} отключен от диска {share.LocalName}";
        }
        catch (Exception e)
        {
            message = $"{nameof(DisconnectShare)} : Ошибка отключения ресурса {share.RemoteName} : {e.Message}";
        }
        finally
        {
            _logger?.WriteFormattedLine(message);
        }
    }

    private static bool IsShareConnected(SharedDisk share)
    {
        var connectedShares = SmbClient.GetConnectedShares();
        return connectedShares.Any(s => s.Equals(share));
    }

    #endregion

    #region Equals

    public bool Equals(SmbServerMonitor? other)
    {
        if (other is null) return false;
        return GetHashCode() == other.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is SmbServerMonitor monitor) return Equals(monitor);
        
        return _hashObject.Equals(obj);
    }

    public override int GetHashCode()
    {
        return _hashObject switch
        {
            Host host => host.GetHashCode(),
            WifiApMonitor wifiApMonitor => wifiApMonitor.GetHashCode(),
            _ => _hashObject.GetHashCode()
        };
    }

    #endregion

    #region Dispose

    private bool _disposed;

    ~SmbServerMonitor()
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
            _portScannerTimer?.Dispose();
            _wifiApMonitor?.Dispose();
        }
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null
        _disposed = true;
    }

    #endregion
}