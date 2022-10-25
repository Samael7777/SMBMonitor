namespace SmbMonitorLib.Services.Base;

public abstract class ControlledService<TService> : BaseService<TService>
{ 
    public bool IsStarted { get; protected set; }

    public void Start()
    {
        if (IsStarted) return;
        IsStarted = true;
        LogWriteLine("Служба запущена.");
        OnStart();
    }

    public void Stop()
    {
        if (!IsStarted) return;
        IsStarted = false;
        LogWriteLine("Служба остановлена.");
        OnStop();
    }

    protected abstract void OnStart();
    protected abstract void OnStop();
}