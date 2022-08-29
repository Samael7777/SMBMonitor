namespace SmbMonitorLib.PortChecking;

/// <summary>
///     Сканер порта по таймеру
/// </summary>
public class PortCheckerOnTimer : IDisposable
{
    private readonly Host _host;
    private readonly int _timeout;
    private readonly Timer _timer;
    private readonly Action _onAccessible;
    private readonly Action _onInaccessible;

    private bool _isLastAccessible;
    private PortChecker? _scanner;


    public PortCheckerOnTimer(CheckerSettings settings)
    {
        _isLastAccessible = false;
        _host = settings.Host;
        _timeout = settings.Timeout;
        _onAccessible = settings.OnPortAccessible;
        _onInaccessible = settings.OnPortInaccessible;
        _timer = new Timer
            (
                TimerCallbackProc, 
                null, 
                0, 
                settings.Interval
            );
    }



    private void TimerCallbackProc(object? args)
    {
        _scanner = new PortChecker(_host, _timeout);
        if (_scanner.IsPortOpen())
        {
            if (_isLastAccessible) return;
            _isLastAccessible = true;
            _onAccessible.Invoke();
        }
        else
        {
            if (!_isLastAccessible) return;
            _isLastAccessible = false;
            _onInaccessible.Invoke();
        }
    }

    #region Dispose

    private bool _disposed;

    ~PortCheckerOnTimer()
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
            _timer.Dispose();
            _scanner = null;
        }

        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null
        _disposed = true;
    }

    #endregion
}