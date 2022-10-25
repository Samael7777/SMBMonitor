namespace SmbMonitorLib.Exceptions;

public class NotLetterException : LetterManagerException
{
    public NotLetterException()
    {
    }

    public NotLetterException(string message) : base(message)
    {
    }
}