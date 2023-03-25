namespace SmbMonitor.Base.Exceptions.SMB;

public class SmbConnectionException : SmbServiceException
{
    public SmbConnectionException(string? message, Exception? innerException) 
        : base(message, innerException)
    { }

    public SmbConnectionException()
    { }

    public SmbConnectionException(string? message) : base(message)
    { }
}
