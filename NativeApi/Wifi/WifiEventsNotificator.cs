using System.Runtime.InteropServices;
using NativeApi.Wifi.Win32;
using NativeApi.Wifi.Win32.DTO;
using WifiAPI.Win32.DTO;
using static NativeApi.Wifi.Win32.WifiAPI;

namespace NativeApi.Wifi;

public enum AdapterState
{
    Unknown, Connected, Disconnected 
}

public delegate void AdapterStateHandler(Guid adapter, AdapterState state);

public class WifiEventsNotificator : IDisposable
{
    private readonly SafeWifiHandle _handle;
    private readonly NotificationCallbackProc _notificationCallbackProc;

    public event AdapterStateHandler? OnAdapterStateChanged;

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
                OnAdapterStateChanged?.Invoke(notificationData.InterfaceGuid, AdapterState.Connected);
                break;
            }
            case WlanNotificationAcm.WlanNotificationAcmDisconnected:
            case WlanNotificationAcm.WlanNotificationAcmInterfaceRemoval:
            {
                OnAdapterStateChanged?.Invoke(notificationData.InterfaceGuid, AdapterState.Disconnected);
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
        {
            //dispose managed state (managed objects)
            _handle.Dispose();
        } 
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null
        OnAdapterStateChanged = null;
        _disposed = true;
    }

    #endregion
}