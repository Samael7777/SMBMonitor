using System.Net;

namespace SmbMonitorLib.Wifi;

/// <summary>
///     Монитор точки доступа WIFI
/// </summary>
public class WifiApMonitor : IEquatable<WifiApMonitor>, IDisposable
{
    private readonly WifiClient _client;
    private NetworkAdapter? _usingAdapter;

    public Action<IPAddress>? OnAPConnected;
    public Action? OnAPDisconnected;

    /// <summary>
    ///     Монитор точки доступа WIFI
    /// </summary>
    /// <param name="ap">Идентификатор отслеживаемой точки доступа</param>

    public WifiApMonitor(WifiNetworkIdentifier ap)
    {
        MonitoringAccessPoint = ap;
        _client = new WifiClient();
        _client.OnAdapterConnected = OnAccessPointConnected;
        _client.OnAdapterDisconnected = OnAccessPointDisconnected;
        _usingAdapter = null;
    }

    #region Propetries

    public WifiNetworkIdentifier MonitoringAccessPoint { get; }
    public bool IsAPConnected { get; private set; }
    public bool IsRunning { get; private set; }

    #endregion

    #region Controls

    public void Start()
    {
        _client.StartWifiNotifications();
        IsRunning = true;
        CheckConnectionOnInit();
    }

    public void Stop()
    {
        _client.StopWifiNotifications();
        IsRunning = false;
    }

    private void CheckConnectionOnInit()
    {
        var adapterId = _client.GetAdapterIdWithNetwork(MonitoringAccessPoint);
        if (!adapterId.Equals(Guid.Empty))
        {
            _usingAdapter = new NetworkAdapter(adapterId);
            var apAddress = _usingAdapter.GatewayIp;

            OnAPConnected?.Invoke(apAddress);
        }
        else
        {
            OnAPDisconnected?.Invoke();
        }
    }

    #endregion

    private void OnAccessPointConnected(Guid adapterId)
    {
        if (adapterId.Equals(Guid.Empty))
            return;

        var connectedNetwork = _client.GetNetworkByAdapterGuid(adapterId);
        if (connectedNetwork is null)
            return;

        if (!connectedNetwork.Ssid.SequenceEqual(MonitoringAccessPoint.ToBytes()))
            return;

        _usingAdapter = new NetworkAdapter(adapterId);
        IsAPConnected = true;

        OnAPConnected?.Invoke(_usingAdapter.GatewayIp);
    }
    private void OnAccessPointDisconnected(Guid adapterId)
    {
        if (_usingAdapter is null) return;
        if (!_usingAdapter.Id.Equals(adapterId)) return;

        _usingAdapter = null;
        IsAPConnected = false;

        OnAPDisconnected?.Invoke();
    }

    #region Equals
    public bool Equals(WifiApMonitor? other)
    {
        if (other is null) return false;
        return ReferenceEquals(this, other) || MonitoringAccessPoint.Equals(other.MonitoringAccessPoint);
    }
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((WifiApMonitor)obj);
    }
    public override int GetHashCode()
    {
        return MonitoringAccessPoint.GetHashCode();
    }
    #endregion

    #region Dispose
    private bool _disposed;

    ~WifiApMonitor()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            //dispose managed state (managed objects)
            _client.Dispose();
        }

        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null
        _disposed = true;
    }

    #endregion
}