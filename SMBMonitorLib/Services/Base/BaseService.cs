using Base;
using SmbMonitorLib.Services.Interfaces;

namespace SmbMonitorLib.Services.Base;

public abstract class BaseService<TService>
{
    public ILogger? Logger { get; set; }

    protected void LogWriteLine(string message)
    {
        var prefix = typeof(TService).Name;
        Logger?.WriteFormattedLine($"{prefix} : {message}");
    }
}