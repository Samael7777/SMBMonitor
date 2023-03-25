using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using SmbMonitor.NativeApi.MessageCapture.Win32;
using static SmbMonitor.NativeApi.NativeErrors.Win32ErrorChecker;
using static SmbMonitor.NativeApi.MessageCapture.Win32.NativeUser32;

namespace SmbMonitor.NativeApi.MessageCapture;

public delegate void WindowsMessageHandler(uint msg, IntPtr wParam, IntPtr lParam);

public class MessageCapture : IDisposable
{
    private const string WndClass = "CaptureWndClass";
    private readonly CallbackProc _wndProcPtr;
    private WindowClassEx _windowClass;
    private SafeWndClassHandler _wndClassHandle;

    public event WindowsMessageHandler? OnWindowsMessageCapture;

    public SafeWndHandle? CaptureWndHandle { get; private set; }

    public MessageCapture()
    {
        _wndProcPtr = WndProc;

        var instanceHandler = GetModuleHandle(null);
        
        if (instanceHandler == IntPtr.Zero)
            CheckLastErrorWithException(nameof(GetModuleHandle));

        RegisterCaptureWindowClass(instanceHandler);

        Task.Run(CaptureTask);
        while(CaptureWndHandle is null){}
    }
    
    private void CaptureTask()
    {
        CaptureWndHandle = CreateWindowEx(
            0, 
            (ushort)_wndClassHandle.DangerousGetHandle().ToInt32(), 
            "", 
            0, 
            0, 0, 0, 0, 
            IntPtr.Zero, 
            IntPtr.Zero, 
            _windowClass.hInstance, 
            IntPtr.Zero);

        if (CaptureWndHandle.IsInvalid)
            CheckLastErrorWithException(nameof(CreateWindowEx));
        
        var error = 1;
        while (error != 0)
        {
            error = GetMessage(out var msg, IntPtr.Zero, 0, 0);
            if (error == -1)
            {
                CheckLastErrorWithException(nameof(GetMessage));
            }
            else
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
        }
        UnregisterCaptureWindowClass();
    }
    private IntPtr WndProc(IntPtr wnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        switch ((WindowsMessage)msg)
        {
            case WindowsMessage.Quit:
                break;
            case WindowsMessage.Destroy:
                CaptureWndHandle?.Dispose();
                break;
            case WindowsMessage.DeviceChange:
            default:
                OnWindowsMessageCapture?.Invoke(msg, wParam, lParam);
                break;
        }
        return DefWindowProc(wnd, msg, wParam, lParam);
    }

    [MemberNotNull(nameof(_wndClassHandle))]
    private void RegisterCaptureWindowClass(IntPtr instance)
    {
        _windowClass = new WindowClassEx
        {
            cbSize = Marshal.SizeOf(typeof(WindowClassEx)),
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate(_wndProcPtr),
            lpszClassName = WndClass,
            style = 0,
            hbrBackground = (IntPtr)0,
            cbClsExtra = 0,
            cbWndExtra = 0,
            hInstance = instance,
            hIcon = IntPtr.Zero,
            hIconSm = IntPtr.Zero,
            hCursor = IntPtr.Zero,
            lpszMenuName = null
        };

        _wndClassHandle = RegisterClassEx(ref _windowClass);
        if (_wndClassHandle.IsInvalid)
            CheckLastErrorWithException(nameof(RegisterClassEx));
    }

    private void UnregisterCaptureWindowClass()
    {
        _wndClassHandle.Dispose();
        CheckLastErrorWithException(nameof(UnregisterClass));
    }
    
    #region Dispose

    private bool _disposed;

    ~MessageCapture()
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
            CaptureWndHandle?.Dispose();
        }
        
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null
        _disposed = true;
    }

    #endregion
}