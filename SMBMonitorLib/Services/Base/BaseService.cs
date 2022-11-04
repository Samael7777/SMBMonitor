using SmbMonitorLib.Exceptions;

namespace SmbMonitorLib.Services.Base;

public abstract class BaseService<TService>
{
    private static TService? _instance;

    public static TService Instance
    {
        get
        {
            if (_instance == null) throw new InitializeException($"{nameof(ServiceController)}.{nameof(ServiceController.BuildDependencies)}");
            return _instance;
        }
    }
    
    public ILogger? Logger { get; set; }

    protected static void SetInstance(TService instance)
    {
        _instance ??= instance;
    }

    protected static bool IsNotInitialized()
    {
        return _instance == null;
    }

    protected void LogWriteLine(string message)
    {
        var prefix = typeof(TService).Name;
        Logger?.WriteFormattedLine($"{prefix} : {message}");
    }
}