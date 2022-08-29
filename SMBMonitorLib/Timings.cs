namespace SmbMonitorLib;

/// <summary>
///     Класс параметров мониторинга
/// </summary>
public class Timings
{
    /// <summary>
    ///     Интервал опроса порта, мс
    /// </summary>
    public int Interval { get; set; }
    /// <summary>
    ///     Таймаут опроса порта, мс
    /// </summary>
    public int Timeout { get; set; }
}
