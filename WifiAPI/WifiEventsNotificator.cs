using System.Runtime.InteropServices;
using WifiAPI.Win32;
using static WifiAPI.Win32.WifiAPI;

namespace WifiAPI;

public class WifiEventsNotificator : IDisposable
{
    private readonly SafeWifiHandle _handle;
    private readonly NotificationCallbackProc _notificationCallbackProc;

    public Action<Guid>? OnAdapterConnected;
    public Action<Guid>? OnAdapterDisconnected;

    public WifiEventsNotificator()
    {
        _handle = OpenHandle();
        _notificationCallbackProc = OnNotificationReceived;
        
    }

    #region Events

    public void StartWifiNotifications()
    {
        RegisterNotification(_handle, _notificationCallbackProc);
    }

    public void StopWifiNotifications()
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

    ~WifiEventsNotificator()
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
            //dispose managed state (managed objects)
            if (!_handle.IsInvalid)
                _handle.Dispose();

        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null
        _disposed = true;
    }

    #endregion
}