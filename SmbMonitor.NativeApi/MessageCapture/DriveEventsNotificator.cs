using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using SmbMonitor.Base.Interfaces.Tools;
using SmbMonitor.NativeApi.MessageCapture.Win32;
using static SmbMonitor.NativeApi.MessageCapture.Win32.NativeUser32;
using static SmbMonitor.NativeApi.NativeErrors.Win32ErrorChecker;

namespace SmbMonitor.NativeApi.MessageCapture;

#region Internal Enums

internal enum DeviceBroadcast
{
    DeviceArrival = 0x8000,
    DeviceRemoveComplete = 0x8004,
}

internal enum DeviceType
{
    DeviceInterface = 0x00000005,
    Volume = 0x02
}

[Flags]
internal enum DeviceNotifyFlag
{
    AllInterfaceClasses = 0x00000004,
    WindowHandle = 0
}

#endregion

public class DriveEventsNotificator : IDriveEventsNotificator, IDisposable
{
    private MessageCapture? _messageCapture;
    private SafeDevNotifyHandle? _deviceNotifyHandle;

    public event DriveStateHandler? OnDriveStateChange;

    //todo make injection of message capture
    public void StartNotification()
    {
        if (_messageCapture is not null) return;

        _messageCapture = new MessageCapture();

        if (_messageCapture.CaptureWndHandle is null
            || _messageCapture.CaptureWndHandle.IsInvalid)
        {
            throw new Win32Exception(nameof(StartNotification));
        }

        _messageCapture.OnWindowsMessageCapture += OnMessageCapture;
        
        RegisterNotifications(_messageCapture.CaptureWndHandle);
    }

    public void StopNotification()
    {
        if (_messageCapture is null) return;

        _messageCapture.OnWindowsMessageCapture -= OnMessageCapture;
        _deviceNotifyHandle?.Dispose();
        _messageCapture?.Dispose();
        _messageCapture = null;
    }

    [MemberNotNull(nameof(_deviceNotifyHandle))]
    private void RegisterNotifications(SafeWndHandle captureWndHandle)
    {
        var bdi = new BroadcastDeviceInterface
        {
            Size = Marshal.SizeOf(typeof(BroadcastDeviceInterface)),
            DeviceType = (int)DeviceType.DeviceInterface
        };
        
        _deviceNotifyHandle = RegisterDeviceNotification(captureWndHandle, ref bdi, 
            (int)(DeviceNotifyFlag.WindowHandle | DeviceNotifyFlag.AllInterfaceClasses));

        if(_deviceNotifyHandle.IsInvalid)
            CheckLastErrorWithException(nameof(RegisterNotifications));
    }

    private void OnMessageCapture(uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg != (uint)WindowsMessage.DeviceChange) return;
        
        var header = Marshal.PtrToStructure<DevBroadcastHeader>(lParam);
        if (header.DeviceType != (int)DeviceType.Volume) return;
        
        var volumeInfo = Marshal.PtrToStructure<DevBroadcastVolume>(lParam);
        
        var driveChar = BitToLetter(volumeInfo.UnitMask);
        var driveLetter = $"{driveChar}:";
        var rootPath = $"{driveLetter}\\";
        var driveType = GetDriveType(rootPath);
        var driveState =wParam.ToInt32() switch
        {
            (int)DeviceBroadcast.DeviceArrival => DriveState.Arrival,
            (int)DeviceBroadcast.DeviceRemoveComplete => DriveState.RemoveComplete,
            _ => DriveState.Unknown
        };
        
        if (driveState == DriveState.Unknown) return;

        OnDriveStateChange?.Invoke(driveLetter, driveType, driveState);
    }

    private static char BitToLetter(uint bits)
    {
        if (bits == 0) return (char)0;

        var offset = (int)Math.Log2(bits);
        var index = 'A' + offset;
        return (char)index;
    }

    #region Dispose

    private bool _disposed;

    ~DriveEventsNotificator()
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
            _deviceNotifyHandle?.Dispose();
            _messageCapture?.Dispose();
        }
        
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null
        
        _disposed = true;
    }

    #endregion
}