using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using NativeApi.MessageCapture.Win32;
using static NativeApi.MessageCapture.Win32.Native;
using static NativeApi.NativeErrors.Win32ErrorChecker;

namespace NativeApi.MessageCapture;

public delegate void DriveStateHandler(char driveLetter,DriveType driveType, DriveState state);

internal enum DeviceBroadcast
{
    DeviceArrival = 0x8000,
    DeviceRemoveComplete = 0x8004
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

public class DriveStateCapture : IDisposable
{
    private SafeDevNotifyHandle _deviceNotifyHandle;

    private readonly MessageCapture _messageCapture;

    public event DriveStateHandler? OnDriveStateChange;
    public DriveStateCapture()
    {
        _messageCapture = new MessageCapture();
        _messageCapture.OnWindowsMessageCapture += OnMessageCapture;
        RegisterNotifications();
    }
    
    [MemberNotNull(nameof(_deviceNotifyHandle))]
    private void RegisterNotifications()
    {
        var bdi = new BroadcastDeviceInterface
        {
            Size = Marshal.SizeOf(typeof(BroadcastDeviceInterface)),
            DeviceType = (int)DeviceType.DeviceInterface
        };
        var wndHandle = _messageCapture.CaptureWndHandle;
        
        if (wndHandle is null || wndHandle.IsInvalid)
            throw new Win32Exception(nameof(RegisterNotifications));

        _deviceNotifyHandle = RegisterDeviceNotification(wndHandle, ref bdi, 
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
        var driveLetter = BitToLetter(volumeInfo.UnitMask);
        var rootPath = $"{driveLetter}:\\";
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

    ~DriveStateCapture()
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
            OnDriveStateChange = null;
            _messageCapture.Dispose();
            _deviceNotifyHandle.Dispose();
        }
        
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null
       _disposed = true;
    }

    #endregion
}