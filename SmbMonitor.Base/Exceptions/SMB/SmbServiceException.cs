namespace SmbMonitor.Base.Exceptions.SMB;

public class SmbServiceException : ServiceException
{
    public SmbServiceException(string? message, Exception? innerException)
        : base(message, innerException)
    { }

    public SmbServiceException()
    { }

    public SmbServiceException(string? message) : base(message)
    { }
}
