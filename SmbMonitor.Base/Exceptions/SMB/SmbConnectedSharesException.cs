namespace SmbMonitor.Base.Exceptions.SMB;

public class SmbConnectedSharesException : SmbServiceException
{
    public SmbConnectedSharesException(string? message, Exception? innerException)
        : base(message, innerException)
    { }

    public SmbConnectedSharesException()
    { }

    public SmbConnectedSharesException(string? message) : base(message)
    { }
}
