namespace Base;

public interface ILogger
{
    public void Write(string message);
    public void WriteFormattedLine(string message);
}