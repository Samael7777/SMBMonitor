using System.Runtime.InteropServices;
using SmbMonitor.Base.Interfaces.Tools;
using SmbMonitor.NativeApi.Wifi.Win32;
using SmbMonitor.NativeApi.Wifi.Win32.DTO;
using static SmbMonitor.NativeApi.Wifi.Win32.WifiAPI;

namespace SmbMonitor.NativeApi.Wifi;

public class WifiEventsNotificator : IWifiEventsNotificator, IDisposable
{
    private SafeWifiHandle? _handle;

    public event AdapterStateHandler? OnAdapterStateChanged;

    public void StartNotification()
    {
        if (_handle is not null) return;

        _handle = OpenHandle();
        RegisterNotification(_handle, OnNotificationReceived);
    }

    public void StopNotification()
    {
        if (_handle is null) return;

        UnregisterNotification(_handle);
        _handle.Dispose();
        _handle = null;
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

        //free unmanaged resources (unmanaged objects) and override finalizer
        StopNotification();

        if (disposing)
        {
            //dispose managed state (managed objects)
            _handle?.Dispose();
        } 
        
        //set large fields to null
        
        OnAdapterStateChanged = null;
        _disposed = true;
    }
    
    #endregion
}