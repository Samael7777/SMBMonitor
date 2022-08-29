namespace SmbMonitorLib;

public interface ILogger
{
    public void Write(string message);
    public void WriteFormattedLine(string message);
    public void SetPrefix(string prefix);
}
