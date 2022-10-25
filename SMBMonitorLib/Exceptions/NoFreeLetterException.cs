namespace SmbMonitorLib.Exceptions;

public class NoFreeLetterException : LetterManagerException
{
    public NoFreeLetterException()
    {
    }

    public NoFreeLetterException(string message) : base(message)
    {
    }
}