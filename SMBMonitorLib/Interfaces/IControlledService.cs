namespace SmbMonitorLib.Interfaces;

public interface IControlledService
{
    bool IsStarted { get; }
    void Start();
    void Stop();
}