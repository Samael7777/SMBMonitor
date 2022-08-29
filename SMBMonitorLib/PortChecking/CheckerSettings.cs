namespace SmbMonitorLib.PortChecking;

public class CheckerSettings
{
    /// <summary>
    ///     Интервал опроса порта, мс
    /// </summary>
    public int Interval { get; set; }
    /// <summary>
    ///     Таймаут опроса порта, мс
    /// </summary>
    public int Timeout { get; set; }
    public Host Host { get; set; }
    public Action OnPortAccessible { get; set; }
    public Action OnPortInaccessible { get; set; }

    public CheckerSettings()
    {
        Host = new Host();
        OnPortAccessible = () => { };
        OnPortInaccessible = () => { };
    }

}
