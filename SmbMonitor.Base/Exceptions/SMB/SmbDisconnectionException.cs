namespace SmbMonitor.Base.Exceptions.SMB;

public class SmbDisconnectionException : SmbServiceException
{
    public SmbDisconnectionException(string? message, Exception? innerException) 
        : base(message, innerException)
    { }

    public SmbDisconnectionException()
    { }

    public SmbDisconnectionException(string? message) : base(message)
    { }
}
