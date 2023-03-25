namespace SmbMonitor.Base.Exceptions.SMB;

public class SmbServerSharesException : SmbServiceException
{
    public SmbServerSharesException(string? message, Exception? innerException)
        : base(message, innerException)
    { }

    public SmbServerSharesException()
    { }

    public SmbServerSharesException(string? message) : base(message)
    { }
}
