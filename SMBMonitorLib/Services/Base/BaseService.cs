using SmbMonitorLib.Interfaces;

namespace SmbMonitorLib.Services.Base;

public abstract class BaseService<TService> : IBaseService
{
    public ILogger? Logger { get; set; }

    protected void LogWriteLine(string message)
    {
        var prefix = typeof(TService).Name;
        Logger?.WriteFormattedLine($"{prefix} : {message}");
    }
}