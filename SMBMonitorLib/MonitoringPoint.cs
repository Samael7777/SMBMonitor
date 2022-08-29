namespace SmbMonitorLib;

[Serializable]
public class MonitoringPoint
{
    public Credentials Credentials { get; set; }
    public object MonitoringObject { get; set; }

    public MonitoringPoint()
    {
        Credentials = new Credentials();
        MonitoringObject = new object();
    }
}
