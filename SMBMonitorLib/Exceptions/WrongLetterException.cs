namespace SmbMonitorLib.Exceptions;

public class WrongLetterException : Exception
{
    public WrongLetterException()
    {
    }

    public WrongLetterException(string message) : base(message)
    {
    }
}