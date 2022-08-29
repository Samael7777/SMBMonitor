using System.Runtime.InteropServices;
using SmbMonitorLib.Wifi.Win32;
using static SmbMonitorLib.Wifi.Win32.WifiBase;

namespace SmbMonitorLib.Wifi;

/// <summary>
///     Класс клиента беспроводной сети
/// </summary>
public class WifiClient : IDisposable
{
    private readonly SafeClientHandle _handle;
    private readonly NotificationCallbackProc _notificationCallbackProc;

    public Action<Guid>? OnAdapterConnected;
    public Action<Guid>? OnAdapterDisconnected;

    public WifiClient()
    {
        OpenHandle(out _handle);
        _notificationCallbackProc = OnNotificationReceived;
    }

    #region Adapters
    
    /// <summary>
    ///     Возвращает список доступных интерфейсов
    /// </summary>
    public List<InterfaceInfo> GetWirelessAdapters()
    {
        return GetInterfaceInfoList(_handle)
            .Select(i => new InterfaceInfo(i)).ToList();
    }

    /// <summary>
    ///     Возвращает GUID адаптера с требуемой подключенной сетью
    /// </summary>
    /// <param name="wifiNetwork">Идентификатор сети</param>
    /// <returns>GUID адаптера</returns>
    public Guid GetAdapterIdWithNetwork(WifiNetworkIdentifier wifiNetwork)
    {
        var networks = GetConnectedNetworks();
        var result = networks.FirstOrDefault(cn => 
                cn.Ssid.SequenceEqual(wifiNetwork.ToBytes()))?.Interface.Id;
        
        return result ?? Guid.Empty;
    }
    
    /// <summary>
    ///     Сканирует эфир заданного адаптера
    /// </summary>
    /// <param name="adapterId">GUID адаптера</param>
    public void Scan(Guid adapterId)
    {
        WifiBase.Scan(_handle, adapterId);
    }

    /// <summary>
    ///     Сканирует эфир всех доступных адаптеров
    /// </summary>
    public void ScanOnAllAdapters()
    {
        List<Task> scanTasks = new();
        foreach (var adapter in GetWirelessAdapters())
        {
            Task scan = new(() => { Scan(adapter.Id); });
            scanTasks.Add(scan);
            scan.Start();
        }

        Task.WaitAll(scanTasks.ToArray());
    }

    #endregion

    #region Networks

    /// <summary>
    ///     Возвращает список подключенных беспроводных сетей
    /// </summary>
    public List<AvailableNetwork> GetConnectedNetworks()
    {
        var networks = GetAvailableNetworks();
        var result = networks
            .Where(net => net.Flags.Connected);
        
        return result.ToList();
    }

    /// <summary>
    ///     Возвращает список доступных беспроводных сетей
    /// </summary>
    public List<AvailableNetwork> GetAvailableNetworks()
    {
        var result = new List<AvailableNetwork>();

        foreach (var adapter in GetWirelessAdapters())
        {
            var availableNetworks = WifiBase
                .GetAvailableNetworks(_handle, adapter.Id)
                .Select(n => new AvailableNetwork(n, adapter))
                .ToList();
            result.AddRange(availableNetworks);
        }

        return result;
    }

    /// <summary>
    ///     Возвращает идентификатор сети, подключенной к адаптеру
    /// </summary>
    /// <param name="adapterId">GUID адаптера</param>
    /// <returns></returns>
    public AvailableNetwork? GetNetworkByAdapterGuid(Guid adapterId)
    {
        return GetConnectedNetworks()
            .FirstOrDefault(cn => cn.Interface.Id.Equals(adapterId));
    }
    #endregion

    #region Events

    /// <summary>
    ///     Запуск обработчика событий сети
    /// </summary>
    public void StartWifiNotifications()
    {
        RegisterNotification(_handle, _notificationCallbackProc);
    }
    
    /// <summary>
    ///     Остановка обработчика событий сети
    /// </summary>
    internal void StopWifiNotifications()
    {
        UnregisterNotification(_handle);
    }

    private void OnNotificationReceived(IntPtr data, IntPtr context)
    {
        var notificationData = Marshal.PtrToStructure<WlanNotificationData>(data);
        if (notificationData.NotificationSource != (int)WlanNotificationSource.Acm)
            return;
        var notificationCode = (WlanNotificationAcm)notificationData.NotificationCode;

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (notificationCode)
        {
            case WlanNotificationAcm.WlanNotificationAcmConnectionComplete:
            {
                OnAdapterConnected?.Invoke(notificationData.InterfaceGuid);
                break;
            }
            case WlanNotificationAcm.WlanNotificationAcmDisconnected:
            case WlanNotificationAcm.WlanNotificationAcmInterfaceRemoval:
            {
                OnAdapterDisconnected?.Invoke(notificationData.InterfaceGuid);
                break;
            }
        }
    }
   
    #endregion
    
    #region Dispose

    private bool _disposed;

    ~WifiClient()
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
        if (!_disposed)
        {
            if (disposing)
                //dispose managed state (managed objects)
                if (!_handle.IsInvalid)
                    _handle.Dispose();

            //free unmanaged resources (unmanaged objects) and override finalizer
            //set large fields to null
            _disposed = true;
        }
    }

    #endregion
}